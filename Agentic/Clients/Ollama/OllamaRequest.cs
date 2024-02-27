using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Agentic.Clients.Ollama
{
    public partial class OllamaRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("messages")]
        public List<OllamaMessage> Messages { get; set; }

        [JsonPropertyName("stream")]
        public bool Stream { get; set; } = false;

        public OllamaRequest()
        {
            Messages = new List<OllamaMessage>();
        }
    }

}
