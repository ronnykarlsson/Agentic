using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Agentic.Clients.OpenAI.API
{
    public class OpenAIClient : IOpenAIClient
    {
        private ILogger _logger { get; set; }

        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(4) };
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public OpenAIClient(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            _apiKey = configuration["OpenAI:ApiKey"] ?? throw new ArgumentNullException("OpenAI:ApiKey");
            _baseUrl = configuration["OpenAI:BaseUrl"] ?? "https://api.openai.com/v1";

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            _logger = loggerFactory?.CreateLogger<OpenAIClient>();
        }

        public OpenAIClient(string apiKey, string baseUrl = "https://api.openai.com/v1")
        {
            if (string.IsNullOrEmpty(apiKey)) throw new ArgumentNullException(nameof(apiKey));
            _apiKey = apiKey;
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<OpenAIResponse> SendRequestAsync(OpenAIRequest request)
        {
            var endpoint = $"{_baseUrl}/chat/completions";

            try
            {
                var payload = new
                {
                    model = request.Model,
                    messages = request.Messages.Select(m => new { role = m.Role, content = m.Content })
                };

                var jsonContent = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(endpoint, content).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<OpenAIResponse>(responseContent);
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

        public async Task<OpenAIEmbeddingResponse> SendEmbeddingsRequestAsync(OpenAIEmbeddingRequest request)
        {
            var endpoint = $"{_baseUrl}/embeddings";

            try
            {
                var payload = new
                {
                    model = request.Model,
                    input = request.Input
                };

                var jsonContent = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(endpoint, content).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<OpenAIEmbeddingResponse>(responseContent);
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
