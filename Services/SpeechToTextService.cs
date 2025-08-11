using System.Net.Http.Headers;
using System.Text.Json;

public class SpeechToTextService
{
    private readonly HttpClient _httpClient;
    private readonly string _openAiApiKey;

    public SpeechToTextService(HttpClient httpClient, string openAiApiKey)
    {
        _httpClient = httpClient;
        _openAiApiKey = openAiApiKey;
    }

    public async Task<string> TranscribeAsync(byte[] audioBytes, string filename = "audio.wav")
    {
        using var content = new MultipartFormDataContent();
        var audioContent = new ByteArrayContent(audioBytes);
        audioContent.Headers.ContentType = new MediaTypeHeaderValue("audio/wav"); 
        content.Add(audioContent, "file", filename);
        content.Add(new StringContent("whisper-1"), "model");

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/audio/transcriptions")
        {
            Content = content
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _openAiApiKey);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("text", out var text))
            return text.GetString() ?? "";

        return "";
    }
}

