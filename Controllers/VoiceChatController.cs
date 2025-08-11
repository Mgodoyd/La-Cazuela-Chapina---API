
using System.Net.WebSockets;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers

{
    [Route("voice")]
    [Authorize]
    public class VoiceChatController : BaseController
    {
        private readonly OpenAIChatService _chat;
        private readonly BusinessContextProvider _contextProvider;
        private readonly SpeechToTextService _speechToText; // Servicio que hace STT 
        private readonly TextToSpeechService _textToSpeech; // Servicio TTS

        public VoiceChatController(OpenAIChatService chat, BusinessContextProvider contextProvider, SpeechToTextService speechToText, TextToSpeechService textToSpeech)
        {
            _chat = chat;
            _contextProvider = contextProvider;
            _speechToText = speechToText;
            _textToSpeech = textToSpeech;
        }

        [HttpGet("/ws/voicechat")]
        public async Task VoiceChat()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = 400;
                return;
            }

            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            var buffer = new byte[8192];
            var conversationHistory = new List<string>();

            while (webSocket.State == WebSocketState.Open)
            {
                var ms = new MemoryStream();
                WebSocketReceiveResult result;
                do
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    ms.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                var audioBytes = ms.ToArray();

                // Transcribe audio a texto
                var spokenText = await _speechToText.TranscribeAsync(audioBytes);

                conversationHistory.Add($"Usuario: {spokenText}");

                var contextText = await _contextProvider.RetrieveContextAsync(spokenText, CancellationToken.None);

                var prompt = string.Join("\n", conversationHistory);
                if (!string.IsNullOrEmpty(contextText))
                    prompt += "\nContexto relevante:\n" + contextText;

                var aiResponse = await _chat.CreateChatAsync(spokenText, null, new[] { contextText }, strict: true, CancellationToken.None);

                conversationHistory.Add($"IA: {aiResponse}");

                var audioResponse = await _textToSpeech.SynthesizeAsync(aiResponse);

                await webSocket.SendAsync(audioResponse, WebSocketMessageType.Binary, true, CancellationToken.None);
            }

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Cerrando conexión", CancellationToken.None);
        }

    }

}