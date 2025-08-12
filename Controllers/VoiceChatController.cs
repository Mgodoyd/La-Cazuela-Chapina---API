
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("voicechat")]
    public class VoiceChatController : BaseController
    {
        private readonly OpenAIChatService _chat;
        private readonly BusinessContextProvider _contextProvider;
        private readonly SpeechToTextService _speechToText;
        private readonly TextToSpeechService _textToSpeech;

        public VoiceChatController(OpenAIChatService chat, BusinessContextProvider contextProvider, SpeechToTextService speechToText, TextToSpeechService textToSpeech)
        {
            _chat = chat;
            _contextProvider = contextProvider;
            _speechToText = speechToText;
            _textToSpeech = textToSpeech;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessVoiceMessage(IFormFile audio)
        {
            try
            {
                if (audio == null || audio.Length == 0)
                {
                    return BadRequest(new { status = "error", message = "No se recibió archivo de audio" });
                }

                // Leer el archivo de audio
                using var memoryStream = new MemoryStream();
                await audio.CopyToAsync(memoryStream);
                var audioBytes = memoryStream.ToArray();

                // Transcribir audio a texto
                var spokenText = await _speechToText.TranscribeAsync(audioBytes);
                
                if (string.IsNullOrEmpty(spokenText))
                {
                    return BadRequest(new { status = "error", message = "No se pudo transcribir el audio" });
                }

                // Obtener contexto relevante
                var contextText = await _contextProvider.RetrieveContextAsync(spokenText, CancellationToken.None);

                // Generar respuesta del AI
                var aiResponse = await _chat.CreateChatAsync(spokenText, null, new[] { contextText }, strict: true, CancellationToken.None);

                if (string.IsNullOrEmpty(aiResponse))
                {
                    return BadRequest(new { status = "error", message = "No se pudo generar respuesta del AI" });
                }

                // Convertir respuesta del texto a audio
                var audioResponse = await _textToSpeech.SynthesizeAsync(aiResponse);

                if (audioResponse == null || audioResponse.Length == 0)
                {
                    return BadRequest(new { status = "error", message = "No se pudo generar audio de respuesta" });
                }

                // Devolver el audio de respuesta
                return File(audioResponse, "audio/wav", "response.wav");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    status = "error", 
                    message = "Error interno del servidor",
                    details = ex.Message 
                });
            }
        }
    }
}