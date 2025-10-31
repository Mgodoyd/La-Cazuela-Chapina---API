using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Api.Services
{
    public class EmbeddingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public EmbeddingService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private string ResolveApiKey() =>
            _configuration["OpenAI:API_KEY_OPENAI"]
            ?? Environment.GetEnvironmentVariable("API_KEY_OPENAI")
            ?? throw new InvalidOperationException("OpenAI API key not configured. API_KEY_OPENAI.");

        private string ResolveEmbeddingModel() => _configuration["OpenAI:EmbeddingModel"] ?? "text-embedding-3-small";

        public async Task<float[]> CreateEmbeddingAsync(string text, CancellationToken ct = default)
        {
            var client = _httpClientFactory.CreateClient("openai");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ResolveApiKey());
            var payload = new { model = ResolveEmbeddingModel(), input = new[] { text } };
            var req = new HttpRequestMessage(HttpMethod.Post, "v1/embeddings")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            var resp = await client.SendAsync(req, ct);
            resp.EnsureSuccessStatusCode();
            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync(ct));
            var arr = doc.RootElement.GetProperty("data")[0].GetProperty("embedding");
            var list = new List<float>(arr.GetArrayLength());
            foreach (var v in arr.EnumerateArray()) list.Add((float)v.GetDouble());
            return list.ToArray();
        }
    }
}


