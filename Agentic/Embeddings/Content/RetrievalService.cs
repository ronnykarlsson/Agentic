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

        public IEnumerable<SearchResult> RetrieveRelevantDocuments(IEnumerable<string> texts, int topK)
        {
            var allSearchResults = new List<SearchResult>();

            foreach (var text in texts)
            {
                try
                {
                    float[] embedding = _embeddingService.GetEmbedding(text);
                    var searchResults = _embeddingStore.FindClosestDocuments(embedding, topK);
                    allSearchResults.AddRange(searchResults);
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Error retrieving documents for text '{text}': {ex.Message}");
                }
            }

            // Remove duplicates based on Document ID
            var uniqueSearchResults = allSearchResults
                .GroupBy(sr => sr.Id)
                .Select(g => g.First());

            return uniqueSearchResults;
        }
    }
}
