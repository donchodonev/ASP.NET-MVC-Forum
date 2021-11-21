namespace ASP.NET_MVC_Forum.Hubs
{
    using ASP.NET_MVC_Forum.Infrastructure.Extensions;
    using ASP.NET_MVC_Forum.Models.Chat;
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


        public async Task SendMessageToGroup(string senderIdentityId, string receiverIdentityId, string message, long chatId, string senderUsername)
        {
            var persistedMessage = await chatService.PersistMessageAsync(chatId, message, senderUsername);

            var time = persistedMessage
                .CreatedOn
                .AddHours(2) // FOR GMT+2
                .ToString("HH:mm:ss");

            var response = new ChatMessageResponseData(senderUsername, time, message);

            await Clients.Group(senderIdentityId).SendAsync("ReceiveMessage", response);

            await Clients.Group(receiverIdentityId).SendAsync("ReceiveMessage", response);
        }

        public async Task GetHistory(long chatId, string sender, string receiver)
        {
            var messages = await chatService
                .GetLastMessages(chatId)
                .ToListAsync();

            await Clients.Group(sender).SendAsync("ReceiveHistory", messages); //chat history bug fix should be implemented here somewhere

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
}