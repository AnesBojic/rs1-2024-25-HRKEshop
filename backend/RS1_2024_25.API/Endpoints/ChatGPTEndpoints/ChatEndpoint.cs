using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Services;
using Microsoft.AspNetCore.RateLimiting;

namespace RS1_2024_25.API.Endpoints
{
    [ApiController]
    [Route("messagess")]
    [EnableRateLimiting("chatLimiter")] // <--- This is where we activate the rate limiting policy
    public class ChatEndpoint : ControllerBase
    {
        private readonly ChatService _chatService;

        public ChatEndpoint(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest("Message cannot be empty.");

            var reply = await _chatService.GetChatResponseAsync(request.Message);
            return Ok(new { response = reply });
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}
