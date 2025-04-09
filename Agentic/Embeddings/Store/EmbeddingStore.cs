using System.Collections.Generic;
using System.Linq;

namespace Agentic.Embeddings.Store
{
    public class EmbeddingStore : IEmbeddingStore
    {
        private readonly List<Document> _documents = new List<Document>();

        public void AddDocument(Document document)
        {
            _documents.Add(document);
        }

        /// <inheritdoc />
        public List<SearchResult> FindClosestDocuments(float[] queryEmbedding, int limit)
        {
            var documentSimilarities = new List<SearchResult>();

            foreach (var document in _documents)
            {
                double similarity = CosineSimilarity(queryEmbedding, document.Embedding);
                documentSimilarities.Add(new SearchResult(document.Id, similarity, document.Content, document.Metadata));
            }

            var topDocuments = documentSimilarities.OrderByDescending(d => d.Similarity).Take(limit).ToList();

            return topDocuments;
        }

        /// <inheritdoc />
        public Document GetDocumentById(string id)
        {
            return _documents.FirstOrDefault(d => d.Id == id);
        }

        private double CosineSimilarity(float[] vectorA, float[] vectorB)
        {
            double dotProduct = 0;
            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
            }
            return dotProduct;
        }
    }
}
