using System.Collections.Generic;

namespace Agentic.Embeddings.Store
{
    public interface IEmbeddingStore
    {
        /// <summary>
        /// Store document
        /// </summary>
        /// <param name="document">Document to store</param>
        void AddDocument(Document document);

        /// <summary>
        /// Search for closest document using cosine similarity.
        /// </summary>
        /// <param name="queryEmbedding">Query to search for</param>
        /// <param name="limit">Number of documents to return</param>
        /// <returns>Closest document</returns>
        List<SearchResult> FindClosestDocuments(float[] queryEmbedding, int limit);
        
        /// <summary>
        /// Get a document by its ID
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <returns>The document</returns>
        Document GetDocumentById(string id);
    }
}
