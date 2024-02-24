using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Search.Similarities;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Agentic.Datastore
{
    public class LuceneService
    {
        private readonly RAMDirectory _ramDirectory;
        private readonly StandardAnalyzer _analyzer;
        private readonly IndexWriterConfig _indexWriterConfig;

        public int SegmentSize { get; set; } = 1000;
        public int OverlapSize { get; set; } = 50;

        public LuceneService()
        {
            _ramDirectory = new RAMDirectory();
            _analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            _indexWriterConfig = new IndexWriterConfig(LuceneVersion.LUCENE_48, _analyzer)
            {
                Similarity = new BM25Similarity() // Use BM25 similarity model
            };
        }

        public void IndexText(string documentId, string text, Dictionary<string, string> metadata = null)
        {
            var segments = SegmentText(text).ToList();
            using (var writer = new IndexWriter(_ramDirectory, _indexWriterConfig))
            {
                for (int segmentIndex = 0; segmentIndex < segments.Count; segmentIndex++)
                {
                    var (Text, ActualLength) = segments[segmentIndex];
                    var doc = new Document
                    {
                        new StringField("documentId", documentId, Field.Store.YES),
                        new TextField("content", Text, Field.Store.YES),
                        new StringField("segmentIndex", segmentIndex.ToString(), Field.Store.YES),
                        new StoredField("actualLength", ActualLength)
                    };

                    metadata?.ToList().ForEach(entry => doc.Add(new StringField(entry.Key, entry.Value, Field.Store.YES)));

                    writer.AddDocument(doc);
                }
                writer.Commit();
            }
        }

        private IEnumerable<(string Text, int ActualLength)> SegmentText(string text)
        {
            int start = 0;
            while (start < text.Length)
            {
                int end = Math.Min(start + SegmentSize, text.Length);
                int segmentEnd = FindSegmentEnd(text, start, end);
                // Ensure segmentEnd is not beyond text.Length and adjust for overlap correctly
                segmentEnd = Math.Min(segmentEnd, text.Length);
                int actualLength = segmentEnd - start;
                // Prevent negative length values
                actualLength = Math.Max(actualLength, 0);
                yield return (text.Substring(start, Math.Min(segmentEnd - start, text.Length - start)), actualLength);
                start = segmentEnd; // Move start to the beginning of the next segment without overlap
            }
        }

        private int FindSegmentEnd(string text, int start, int end)
        {
            int lastPeriod = text.LastIndexOf('.', end - 1, Math.Min(end - start, text.Length - start));
            if (lastPeriod > start && lastPeriod + 1 < end) return lastPeriod + 1;
            int spaceIndex = text.LastIndexOf(' ', end - 1, Math.Min(end - start, text.Length - start));
            return spaceIndex > start ? spaceIndex + 1 : end;
        }

        public List<SearchResult> SearchText(string searchTerm)
        {
            var booleanQuery = new BooleanQuery();
            var phraseQuery = new PhraseQuery { Boost = 2.0f }; // Boost matches on the entire phrase
            searchTerm.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(term =>
            {
                phraseQuery.Add(new Term("content", term));
                booleanQuery.Add(new FuzzyQuery(new Term("content", term)), Occur.SHOULD); // Include fuzzy matches for each term
            });
            booleanQuery.Add(phraseQuery, Occur.SHOULD); // Add the phrase query to the boolean query

            var results = new List<SearchResult>();
            using (var reader = DirectoryReader.Open(_ramDirectory))
            {
                var searcher = new IndexSearcher(reader) { Similarity = _indexWriterConfig.Similarity };
                var hits = searcher.Search(booleanQuery, 10).ScoreDocs;

                foreach (var hit in hits)
                {
                    var doc = searcher.Doc(hit.Doc);
                    results.Add(new SearchResult
                    {
                        DocumentId = doc.Get("documentId"),
                        ContentSnippet = doc.Get("content").Trim(),
                        Score = hit.Score,
                        Metadata = doc.Fields.Where(f => f.Name != "content" && f.Name != "segmentIndex" && f.Name != "actualLength")
                                             .ToDictionary(f => f.Name, f => doc.Get(f.Name)),
                        SegmentIndex = int.TryParse(doc.Get("segmentIndex"), out int index) ? index : -1
                    });
                }
            }
            return results;
        }

        public string GetFullDocument(string documentId)
        {
            var query = new TermQuery(new Term("documentId", documentId));
            var documentParts = new StringBuilder();
            using (var reader = DirectoryReader.Open(_ramDirectory))
            {
                var searcher = new IndexSearcher(reader) { Similarity = _indexWriterConfig.Similarity };
                var hits = searcher.Search(query, int.MaxValue).ScoreDocs.OrderBy(hit => int.Parse(searcher.Doc(hit.Doc).Get("segmentIndex")));
                foreach (var hit in hits)
                {
                    var doc = searcher.Doc(hit.Doc);
                    documentParts.Append(doc.Get("content").Substring(0, int.Parse(doc.Get("actualLength"))));
                }
            }
            return documentParts.ToString();
        }
    }

    public class SearchResult
    {
        public string DocumentId { get; set; }
        public string ContentSnippet { get; set; }
        public float Score { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public int SegmentIndex { get; set; }

        public SearchResult()
        {
            Metadata = new Dictionary<string, string>();
        }
    }
}
