namespace ASP.NET_MVC_Forum.Hubs
{
    using ASP.NET_MVC_Forum.Services.Chat;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;

    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService chatService;

        public ChatHub(IChatService chatService)
        {
            this.chatService = chatService;
        }

        public override Task OnConnectedAsync()
        {
            Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
            return base.OnConnectedAsync();
        }

        public async Task SendMessageToGroup(string sender, string receiver, string message)
        {
            await Clients.Group(sender).SendAsync("SendMessage", sender, message);
            await Clients.Group(receiver).SendAsync("SendMessage", sender, message);
            return;
        }
    }
}
