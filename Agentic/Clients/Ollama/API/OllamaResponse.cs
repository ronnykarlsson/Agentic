using System.Text.Json.Serialization;

namespace Agentic.Clients.Ollama.API
{
    public class OllamaResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("created_at")]
        public string Created { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("message")]
        public OllamaMessage Message { get; set; }
    }

}
