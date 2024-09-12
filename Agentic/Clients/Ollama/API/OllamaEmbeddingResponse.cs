using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Agentic.Clients.Ollama.API
{
    public class OllamaEmbeddingResponse
    {
        [JsonPropertyName("embedding")]
        public float[] Embedding { get; set; }
    }
}
