using Agentic.Embeddings.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Agentic.Embeddings.Context;

namespace Agentic.Embeddings.Content
{
    public class RetrievalService : IRetrievalService
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly IEmbeddingStore _embeddingStore;

        public RetrievalService(IEmbeddingContext embeddingContext)
        {
            _embeddingService = embeddingContext?.Service ?? throw new ArgumentNullException(nameof(embeddingContext.Service));
            _embeddingStore = embeddingContext?.Store ?? throw new ArgumentNullException(nameof(embeddingContext.Store));
        }

        public IEnumerable<SearchResult> RetrieveRelevantDocuments(IEnumerable<string> texts, int topK, RetrievalOptions options = null)
        {
            var sortedSearchResults = new List<SearchResult>();

            foreach (var text in texts)
            {
                try
                {
                    float[] embedding = _embeddingService.GetEmbedding(text);
                    var searchResults = _embeddingStore.FindClosestDocuments(embedding, topK);

                    foreach (var result in searchResults)
                    {
                        int index = sortedSearchResults.BinarySearch(result, Comparer<SearchResult>.Create((x, y) => string.Compare(x.Id, y.Id, StringComparison.Ordinal)));
                        if (index < 0)
                        {
                            sortedSearchResults.Insert(~index, result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Error retrieving documents for text '{text}': {ex.Message}");
                }
            }

            if (options == null || (options.PrecedingChunks <= 0 && options.FollowingChunks <= 0))
            {
                return sortedSearchResults.Take(topK);
            }
            else
            {
                sortedSearchResults = sortedSearchResults.Take(topK).ToList();
            }

            var additionalResults = new List<SearchResult>();

            foreach (var result in sortedSearchResults.ToArray())
            {
                if (!TryParseDocumentId(result.Id, out string source, out int chunkIndex))
                {
                    continue;
                }

                for (int i = 1; i <= options.PrecedingChunks; i++)
                {
                    int prevChunkIndex = chunkIndex - i;
                    if (prevChunkIndex >= 0)
                    {
                        string prevChunkId = $"{source}:{prevChunkIndex}";
                        var matchingDocument = _embeddingStore.GetDocumentById(prevChunkId);

                        if (matchingDocument != null)
                        {
                            var additionalResult = new SearchResult(matchingDocument.Id, 1.0, matchingDocument.Content, matchingDocument.Metadata);
                            if (!sortedSearchResults.Any(sr => sr.Id == additionalResult.Id))
                            {
                                sortedSearchResults.Add(additionalResult);
                            }
                        }
                    }
                }

                for (int i = 1; i <= options.FollowingChunks; i++)
                {
                    int nextChunkIndex = chunkIndex + i;
                    string nextChunkId = $"{source}:{nextChunkIndex}";
                    var matchingDocument = _embeddingStore.GetDocumentById(nextChunkId);

                    if (matchingDocument != null)
                    {
                        var additionalResult = new SearchResult(matchingDocument.Id, 1.0, matchingDocument.Content, matchingDocument.Metadata);
                        if (!sortedSearchResults.Any(sr => sr.Id == additionalResult.Id))
                        {
                            sortedSearchResults.Add(additionalResult);
                        }
                    }
                }
            }

            return sortedSearchResults;
        }

        private bool TryParseDocumentId(string documentId, out string source, out int chunkIndex)
        {
            source = null;
            chunkIndex = -1;

            if (string.IsNullOrEmpty(documentId))
                return false;

            var parts = documentId.Split(':');
            if (parts.Length != 2)
                return false;

            source = parts[0];
            return int.TryParse(parts[1], out chunkIndex);
        }
    }
}
