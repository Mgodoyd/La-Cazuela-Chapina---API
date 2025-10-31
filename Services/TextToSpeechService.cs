
using System.Text.Json;
using System.Net.Http.Headers;

namespace Api.Services 
{
    public class TextToSpeechService
    {
        private readonly HttpClient _httpClient;
        private readonly string _openAiApiKey;

        public TextToSpeechService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _openAiApiKey = Environment.GetEnvironmentVariable("API_KEY_OPENAI") 
                ?? configuration["OpenAI:ApiKey"] 
                ?? throw new InvalidOperationException("API_KEY_OPENAI no está configurada");
        }

        public async Task<byte[]> SynthesizeAsync(string text)
        {
            var requestBody = new
            {
                model = "tts-1", // Modelo correcto para TTS
                voice = "alloy", // Voces disponibles: alloy, echo, fable, onyx, nova, shimmer
                input = text,
                response_format = "mp3"
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/audio/speech")
            {
                Content = content
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _openAiApiKey);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
