using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatbotBackend.Models
{
    public class ChatSession
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string SessionId { get; set; } = string.Empty;
        public string Query { get; set; } = string.Empty;
        public string Reply { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
