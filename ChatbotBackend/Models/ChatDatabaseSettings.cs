namespace ChatBotAPI.Models
{
    public class ChatDatabaseSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string ChatCollectionName { get; set; } = string.Empty;
        public string ChatSessionsCollectionName { get; set; } = null!;

    }
}
