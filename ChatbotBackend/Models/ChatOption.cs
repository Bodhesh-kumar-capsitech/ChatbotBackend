using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatbotBackend.Models
{
    public class ChatOption
    {
        [BsonElement("label")]
        public string? Label { get; set; }

        [BsonElement("query")]
        public ChatMessage? Query { get; set; }
    }
}

public class Apiresponse<T>
{
    public string Message { get; set; } = string.Empty;

    public bool Status { get; set; } = false;

    public T? Result { get; set; }
}
