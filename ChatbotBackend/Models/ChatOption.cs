using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatbotBackend.Models
{
    public class ChatOption
    {
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public List<ChatOption> Options { get; set; } = new();
    }
}

public class Apiresponse<T>
{
    public string Message { get; set; } = string.Empty;

    public bool Status { get; set; } = false;

    public T? Result { get; set; }
}
