using System.Text.Json.Serialization;

namespace Agentic.Clients.Ollama
{
    public class OllamaMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
