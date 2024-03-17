using System.Collections.Generic;

namespace Agentic.Embeddings.Store
{
    public class Document
    {
        public string Id { get; }
        public string Content { get; }
        public float[] Embedding { get; }
        public Dictionary<string, string> Metadata { get; }

        public Document(string id, string content, float[] embedding, Dictionary<string, string> metadata)
        {
            Id = id;
            Content = content;
            Embedding = embedding;
            Metadata = metadata;
        }
    }
}
