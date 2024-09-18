using Agentic.Embeddings.Store;
using System.Collections.Generic;

namespace Agentic.Embeddings.Content
{
    public interface IRetrievalService
    {
        IEnumerable<SearchResult> RetrieveRelevantDocuments(IEnumerable<string> texts, int topK);
    }
}
