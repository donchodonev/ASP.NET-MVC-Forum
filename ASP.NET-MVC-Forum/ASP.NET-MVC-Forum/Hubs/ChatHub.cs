namespace ASP.NET_MVC_Forum.Web.Hubs
{
    using ASP.NET_MVC_Forum.Business.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;

    using System.Threading.Tasks;

    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService chatService;

        public ChatHub(
            IChatService chatService)
        {
            this.chatService = chatService;
        }


        public async Task SendMessageToGroup(
            string senderIdentityId,
            string receiverIdentityId,
            string message,
            long chatId,
            string senderUsername)
        {
            await chatService
                .SendMessageToClientsAsync(
                senderIdentityId,
                receiverIdentityId,
                message,
                chatId,
                senderUsername,
                Clients);
        }

        public async Task GetHistory(long chatId, string sender, string receiver)
        {
            await chatService.SendHistoryAsync(chatId, sender, receiver, Clients);
        }

        public async Task ConnectUserGroups(string senderId, string recipientId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, senderId + recipientId);
        }

        public async Task DisconnectUserGroups(string senderId, string recipientId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, senderId + recipientId);
        }

        public void Disconnect()
        {
            Dispose();
        }
    }
}