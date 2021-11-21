namespace ASP.NET_MVC_Forum.Hubs
{
    using ASP.NET_MVC_Forum.Infrastructure.Extensions;
    using ASP.NET_MVC_Forum.Services.Chat;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService chatService;

        public ChatHub(IChatService chatService)
        {
            this.chatService = chatService;
        }


        public async Task SendMessageToGroup(string sender, string receiver, string message, long chatId,string senderUsername)
        {
            await chatService.PersistMessageAsync(chatId, message,senderUsername);

            await Clients.Group(sender).SendAsync("ReceiveMessage", new TestMessage { Sender = senderUsername, Text = message });

            await Clients.Group(receiver).SendAsync("ReceiveMessage", new TestMessage { Sender = senderUsername, Text = message });
        }

        public async Task GetHistory(long chatId, string sender, string receiver)
        {
            var messages = await chatService
                .GetLastMessages(chatId)
                .ToListAsync();

            await Clients.Group(sender).SendAsync("ReceiveHistory", messages);

            await Clients.Group(receiver).SendAsync("ReceiveHistory", messages);
        }

        public void Disconnect()
        {
            Dispose();
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.User.Id());

            await base.OnDisconnectedAsync(exception);
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Id());
            await base.OnConnectedAsync();
        }

    }
    public class TestMessage
    {
        public string Sender { get; set; }
        public string Text { get; set; }
    }
}