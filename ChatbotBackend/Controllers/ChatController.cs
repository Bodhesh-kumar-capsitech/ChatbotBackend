using ChatbotBackend.Models;
using ChatbotBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatbotBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

       [HttpGet("reply")]
public async Task<Apiresponse<object>> GetReply(string query, [FromQuery] string? sessionId = null)
{
    var res = new Apiresponse<object>();
    try
    {
        // Generate new session ID if not provided
        sessionId ??= Guid.NewGuid().ToString();

        // Modified GetByQueryAsync to include session logging
        var result = await _chatService.GetByQueryAsync(query, sessionId);

        if (result == null)
        {
            res.Message = "Sorry, I couldn't find an answer for that.";
            res.Status = false;
            res.Result = new
            {
                SessionId = sessionId,
                Reply = "Sorry, I couldn't find an answer for that.",
                Options = Array.Empty<object>()
            };
            return res;
        }

        res.Message = "Reply found successfully.";
        res.Status = true;
        res.Result = new
        {
            SessionId = sessionId,
            Reply = result.Reply,
            Options = result.Options
        };
    }
    catch (Exception ex)
    {
        res.Message = "Error: " + ex.Message;
        res.Status = false;
        res.Result = null;
    }

    return res;
}


    }
}
