using ChatBotAPI.Models;
using ChatbotBackend.Models;
using ChatbotBackend.Services;
using Microsoft.Extensions.Options;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Register settings
builder.Services.Configure<ChatDatabaseSettings>(
    builder.Configuration.GetSection("ChatDatabaseSettings"));

// Register ChatService
builder.Services.AddSingleton<ChatService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed chatbot data from chatbot.json
using (var scope = app.Services.CreateScope())
{
    var chatService = scope.ServiceProvider.GetRequiredService<ChatService>();
    var jsonData = File.ReadAllText("chatbot.json");

    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var messages = JsonSerializer.Deserialize<List<ChatMessage>>(jsonData, options);

    if (messages != null)
    {
        await chatService.SeedAsync(messages);
    }
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();

app.Run();
