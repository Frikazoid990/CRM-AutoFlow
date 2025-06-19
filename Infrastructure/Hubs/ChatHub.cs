using Microsoft.AspNetCore.SignalR;

namespace CRM_AutoFlow.Infrastructure.Hubs
{
    public class ChatHub : Hub
    {
        public async Task JoinToChatAsync(int ChatId)
        {

            Console.WriteLine(ChatId);
            await Groups.AddToGroupAsync(Context.ConnectionId, Convert.ToString(ChatId));
        }

        public async Task SendMessage(int ChatId, string UserName, string message)
        {
            Console.WriteLine($"{UserName}: {message}");
            await Clients.Group(Convert.ToString(ChatId)).SendAsync("ReceiveMessage", $"{UserName}: {message}");
        }
    }
}
