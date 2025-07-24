using ChatBotAPI.Models;
using ChatbotBackend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;


namespace ChatbotBackend.Services
{
    public class ChatService
    {
        private readonly IMongoCollection<ChatMessage> _chatCollection;
        private readonly IMongoCollection<ChatSession> _sessionCollection;

        public ChatService(IOptions<ChatDatabaseSettings> settings)
        {
            var mongoClient = new MongoClient(settings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _chatCollection = mongoDatabase.GetCollection<ChatMessage>(settings.Value.ChatCollectionName);
            _sessionCollection = mongoDatabase.GetCollection<ChatSession>("ChatSessions");
        }

        public async Task<List<ChatMessage>> GetAsync() =>
            await _chatCollection.Find(_ => true).ToListAsync();

        public async Task<ChatMessage> GetByQueryAsync(string query, string sessionId)
        {
            // Generate a sessionId if not provided
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
            }

            // Try direct match first
            var result = await _chatCollection
                .Find(x => x.Query.ToLowerInvariant() == query.ToLowerInvariant())
                .FirstOrDefaultAsync();

            // If not found, search recursively through nested options
            if (result == null)
            {
                var allMessages = await _chatCollection.Find(_ => true).ToListAsync();

                foreach (var message in allMessages)
                {
                    result = SearchNestedQuery(message, query);
                    if (result != null)
                        break;
                }
            }

            var reply = result?.Reply ?? "Sorry, I couldn't find an answer for that.";

            // Log the session
            var filter = Builders<ChatSession>.Filter.Eq(s => s.SessionId, sessionId);

            var customerTurn = new ChatTurn
            {
                Sender = "customer",
                Message = query,
                Timestamp = DateTime.UtcNow
            };

            var botTurn = new ChatTurn
            {
                Sender = "bot",
                Message = reply,
                Timestamp = DateTime.UtcNow
            };

            var update = Builders<ChatSession>.Update.PushEach("Conversation", new[] { customerTurn, botTurn });

            var options = new UpdateOptions { IsUpsert = true };

            try
            {
                await _sessionCollection.UpdateOneAsync(filter, update, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to update session: {ex.Message}");
            }


            return result!;
        }


        private ChatMessage? SearchNestedQuery(ChatMessage message, string query)
        {
            if (message.Query.Equals(query, StringComparison.OrdinalIgnoreCase))
                return message;

            if (message.Options != null)
            {
                foreach (var option in message.Options)
                {
                    var found = SearchNestedQuery(option.Query, query);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }

        public async Task<ChatSession?> GetSessionByIdAsync(string sessionId)
        {
            return await _sessionCollection.Find(s => s.SessionId == sessionId).FirstOrDefaultAsync();
        }


        public async Task SeedAsync(List<ChatMessage> messages)
        {
            var count = await _chatCollection.CountDocumentsAsync(FilterDefinition<ChatMessage>.Empty);
            if (count == 0)
            {
                await _chatCollection.InsertManyAsync(messages);
            }
        }
        public async Task<List<string>> GetAllTopLevelQueriesAsync()
        {
            return await _chatCollection
                .Find(_ => true)
                .Project(x => x.Query)
                .ToListAsync();
        }
    }
}
