using System.Collections.Generic;

namespace Agentic.Embeddings.Store
{
    public class SearchResult
    {
        public string Id { get; }
        public double Similarity { get; }
        public string Content { get; }

        public SearchResult(string id, double similarity, string content)
        {
            Id = id;
            Similarity = similarity;
            Content = content;
        }
    }
}
