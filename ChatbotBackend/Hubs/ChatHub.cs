using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ChatbotBackend.Hubs
{
    public class ChatHub : Hub
    {
        // Called by either user or employee to send message to all participants
        public async Task SendMessage(string sessionId, string sender, string message)
        {
            // Broadcast message to all clients
            await Clients.All.SendAsync("ReceiveMessage", sender, message);
        }

        // Optional: Log when someone connects (for debugging)
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        // Optional: Log when someone disconnects
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
