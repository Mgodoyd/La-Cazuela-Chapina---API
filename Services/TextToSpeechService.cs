using System.Net.Http.Headers;
using System.Text;

public class TextToSpeechService
{
  private readonly HttpClient _httpClient;
  private readonly string _azureSubscriptionKey;
  private readonly string _region;

  public TextToSpeechService(HttpClient httpClient, string azureSubscriptionKey, string region)
  {
    _httpClient = httpClient;
    _azureSubscriptionKey = azureSubscriptionKey;
    _region = region;
  }

  public async Task<byte[]> SynthesizeAsync(string text)
  {
    var uri = $"https://{_region}.tts.speech.microsoft.com/cognitiveservices/v1";

    var ssml = $@"
                  <speak version='1.0' xml:lang='en-US'>
                    <voice xml:lang='en-US' xml:gender='Female' name='en-US-JessaNeural'>
                      {System.Security.SecurityElement.Escape(text)}
                    </voice>
                  </speak>";

    using var request = new HttpRequestMessage(HttpMethod.Post, uri);
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await FetchTokenAsync());
    request.Headers.Add("User-Agent", "YourAppName");
    request.Headers.Add("X-Microsoft-OutputFormat", "audio-16khz-32kbitrate-mono-mp3");
    request.Content = new StringContent(ssml, Encoding.UTF8, "application/ssml+xml");

    var response = await _httpClient.SendAsync(request);
    response.EnsureSuccessStatusCode();

    return await response.Content.ReadAsByteArrayAsync();
  }

  private async Task<string> FetchTokenAsync()
  {
    var tokenUri = $"https://{_region}.api.cognitive.microsoft.com/sts/v1.0/issueToken";
    using var request = new HttpRequestMessage(HttpMethod.Post, tokenUri);
    request.Headers.Add("Ocp-Apim-Subscription-Key", _azureSubscriptionKey);
    var response = await _httpClient.SendAsync(request);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsStringAsync();
  }
}
