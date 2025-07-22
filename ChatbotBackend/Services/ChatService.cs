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
            var result = await _chatCollection.Find(x => x.Query.ToLower() == query.ToLower()).FirstOrDefaultAsync();

            var reply = result?.Reply ?? "Sorry, I couldn't find an answer for that.";

            // Log the session
            var sessionEntry = new ChatSession
            {
                SessionId = sessionId,
                Query = query,
                Reply = reply,
                Timestamp = DateTime.UtcNow
            };

           // await _sessionCollection.InsertOneAsync(sessionEntry);
            try
            {
                await _sessionCollection.InsertOneAsync(sessionEntry);
                Console.WriteLine("Session inserted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to insert session: {ex.Message}");
            }


            return result!;
        }


        public async Task SeedAsync(List<ChatMessage> messages)
        {
            var count = await _chatCollection.CountDocumentsAsync(FilterDefinition<ChatMessage>.Empty);
            if (count == 0)
            {
                await _chatCollection.InsertManyAsync(messages);
            }
        }
    }
}
