using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Agentic.Clients.OpenAI
{
    public class OpenAIEmbeddingResponse
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("data")]
        public List<OpenAIEmbeddingData> Data { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("usage")]
        public OpenAIEmbeddingUsage Usage { get; set; }
    }

    public class OpenAIEmbeddingData
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("embedding")]
        public float[] Embedding { get; set; }
    }

    public class OpenAIEmbeddingUsage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }
}
