using System.Text.Json.Serialization;

namespace Agentic.Clients.Ollama.API
{
    public class OllamaEmbeddingRequest
    {
        public OllamaEmbeddingRequest()
        {
        }

        public OllamaEmbeddingRequest(string model, string prompt)
        {
            Model = model;
            Prompt = prompt;
        }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("prompt")]
        public string Prompt { get; set; }
    }
}
