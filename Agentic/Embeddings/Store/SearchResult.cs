using System.Collections.Generic;

namespace Agentic.Embeddings.Store
{
    public class SearchResult
    {
        public string Id { get; }
        public double Similarity { get; }
        public string Content { get; }
        public Dictionary<string, string> Metadata { get; }

        public SearchResult(string id, double similarity, string content, Dictionary<string, string> metadata = null)
        {
            Id = id;
            Similarity = similarity;
            Content = content;
            Metadata = metadata ?? new Dictionary<string, string>();
        }
    }
}
