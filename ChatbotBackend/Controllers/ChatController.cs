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
                // Ensure sessionId is valid (generate only if empty or whitespace)
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    sessionId = Guid.NewGuid().ToString();
                }

                // Get the response using chat service
                var result = await _chatService.GetByQueryAsync(query, sessionId);

                if (result == null)
                {
                    res.Message = "Sorry, I couldn't find an answer for that.";
                    res.Status = false;
                    res.Result = new
                    {
                        SessionId = sessionId,
                        Answer = "Sorry, I couldn't find an answer for that.",
                        Options = Array.Empty<object>()
                    };  
                    return res;
                }

                res.Message = "Reply found successfully.";
                res.Status = true;
                res.Result = new
                {
                    SessionId = sessionId,
                    Reply = result.Answer,
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

        [HttpPost("history")]
        public async Task<Apiresponse<object>> GetSessionHistory([FromBody] SessionRequest request)
        {
            var res = new Apiresponse<object>();

            try
            {
                if (string.IsNullOrWhiteSpace(request.SessionId))
                {
                    res.Message = "Session ID is required.";
                    res.Status = false;
                    return res;
                }

                var session = await _chatService.GetSessionByIdAsync(request.SessionId);

                if (session == null)
                {
                    res.Message = "Session not found.";
                    res.Status = false;
                    return res;
                }

                res.Message = "Conversation history retrieved successfully.";
                res.Status = true;
                res.Result = new
                {
                    SessionId = session.SessionId,
                    Conversation = session.Conversation.OrderBy(c => c.Timestamp)
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


        [HttpGet("start")]
        public async Task<Apiresponse<object>> StartChat()
        {
            var res = new Apiresponse<object>();

            try
            {
                string sessionId = Guid.NewGuid().ToString();

                var rootQueries = await _chatService.GetAllTopLevelQueriesAsync();

                res.Status = true;
                res.Message = "Session started";
                res.Result = new
                {
                    sessionId,
                    reply = "Hi! Hou Can I help You?",
                    defaultQueries = rootQueries
                };
            }
            catch (Exception ex)
            {
                res.Status = false;
                res.Message = "Error starting chat";
                res.Result = ex.Message;
            }

            return res;
        }




    }
}
