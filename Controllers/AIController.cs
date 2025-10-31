using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("ai")]
    public class AIController : BaseController
    {
        private readonly OpenAIChatService _chat;
        private readonly BusinessContextProvider _contextProvider;

        public AIController(OpenAIChatService chat, BusinessContextProvider contextProvider)
        {
            _chat = chat;
            _contextProvider = contextProvider;
        }

        [HttpPost("chat")]
        [Authorize]
        public Task<IActionResult> Chat([FromBody] ChatRequest req) => ExecuteAsync(async () =>
        {
            var contextText = await _contextProvider.RetrieveContextAsync(req.Message, ct: HttpContext.RequestAborted);

            var contextChunks = !string.IsNullOrWhiteSpace(contextText)
                ? contextText.Split(new[] { "\n---\n" }, StringSplitOptions.RemoveEmptyEntries)
                : Array.Empty<string>();

            var text = await _chat.CreateChatAsync(req.Message, null, contextChunks, strict: true, HttpContext.RequestAborted);
            return Ok(new { status = "ok", data = text });
        });


        // SSE streaming endpoint: Content-Type: text/event-stream
        [HttpPost("stream")]
        [Authorize]
        public async Task Stream([FromBody] ChatRequest req)
        {
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("X-Accel-Buffering", "no");

            var context = await _contextProvider.RetrieveContextAsync(req.Message, ct: HttpContext.RequestAborted);
            await foreach (var token in _chat.StreamChatAsync(req.Message, null, new[] { context }, strict: true, HttpContext.RequestAborted))
            {
                await Response.WriteAsync($"data: {token}\n\n");
                await Response.Body.FlushAsync();
            }

            await Response.WriteAsync("data: [DONE]\n\n");
        }
    }

    public record ChatRequest(string Message);
}
