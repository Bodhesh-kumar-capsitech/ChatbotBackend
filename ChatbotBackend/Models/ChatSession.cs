using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatbotBackend.Models
{
    public class ChatTurn
    {
        [BsonElement("sender")]
        public string Sender { get; set; } = string.Empty;

        [BsonElement("message")]
        public string Message { get; set; } = string.Empty;

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }
    }

    public class ChatSession
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("SessionId")]
        public string SessionId { get; set; } = string.Empty;

        [BsonElement("Humanchat")]
        public bool IsHumanChat { get; set; } = false;


        [BsonElement("Conversation")]
        public List<ChatTurn> Conversation { get; set; } = new();
    }
}
