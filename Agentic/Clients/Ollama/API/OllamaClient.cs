using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Agentic.Clients.Ollama.API
{
    public class OllamaClient : IOllamaClient
    {
        private ILogger _logger { get; set; }

        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public OllamaClient(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            _apiKey = configuration["Ollama:ApiKey"] ?? "";
            _baseUrl = configuration["Ollama:BaseUrl"] ?? "http://127.0.0.1:11434";

            if (!string.IsNullOrEmpty(_apiKey))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            }

            _logger = loggerFactory?.CreateLogger<OllamaClient>();
        }

        public OllamaClient(string apiKey, string baseUrl = "http://127.0.0.1:11434")
        {
            _apiKey = apiKey;
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));

            if (!string.IsNullOrEmpty(_apiKey))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            }
        }

        public async Task<OllamaResponse> SendRequestAsync(OllamaRequest request)
        {
            var endpoint = $"{_baseUrl}/api/chat";

            try
            {
                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(endpoint, content).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<OllamaResponse>(responseContent);
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogError($"HTTP Request failed: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError($"An unexpected error occurred: {ex.Message}");
                throw;
            }
        }

        public async Task<float[]> GetEmbeddingsAsync(string input, string model)
        {
            var endpoint = $"{_baseUrl}/api/embeddings";

            try
            {
                var request = new OllamaEmbeddingRequest(model, input);
                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(endpoint, content).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var ollamaResponse = JsonSerializer.Deserialize<OllamaEmbeddingResponse>(responseContent);
                return ollamaResponse.Embedding;
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogError($"HTTP Request failed: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError($"An unexpected error occurred: {ex.Message}");
                throw;
            }
        }
    }
}
