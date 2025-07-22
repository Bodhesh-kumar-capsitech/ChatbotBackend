using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatbotBackend.Models
{
    public class ChatMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string SessionId { get; set; } = string.Empty;

        public string Query { get; set; } = string.Empty;
        public string Reply { get; set; } = string.Empty;
        public List<ChatOption> Options { get; set; } = new();
    }

   
}
