using ChatBotAPI.Models;
using ChatbotBackend.Models;
using ChatbotBackend.Services;
using Microsoft.Extensions.Options;
using System.Text.Json;
using ChatbotBackend.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Register ChatDatabase settings
builder.Services.Configure<ChatDatabaseSettings>(
    builder.Configuration.GetSection("ChatDatabaseSettings"));

// Register ChatService
builder.Services.AddSingleton<ChatService>();

// Add controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS policy for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // your frontend origin
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for SignalR
    });
});

// Register SignalR
builder.Services.AddSignalR();

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

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Correct middleware order is CRUCIAL for SignalR + CORS
app.UseRouting();                  // ✅ REQUIRED before UseCors
app.UseCors("AllowFrontend");     // ✅ SignalR needs this after UseRouting
app.UseAuthorization();

// SignalR and API routing
app.MapHub<ChatHub>("/chathub");  // ✅ must come after routing + cors
app.MapControllers();

app.Run();
