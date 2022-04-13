namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Models.Chat;

    using Microsoft.AspNetCore.SignalR;

    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IChatService
    {
        public Task<ChatSelectUserViewModel> GenerateChatSelectUserViewModel(
            string recipientUsername,
            string currentIdentityUserId,
            string currentIdentityUserUsername);

        public Task<ChatConversationViewModel> GenerateChatConversationViewModel(
            string senderId,
            string recipientId,
            string senderUsername);

        public Task SendMessageToClientsAsync(
            string senderId,
            string receiverId,
            string message,
            long chatId,
            string senderUsername,
            IHubCallerClients clients);

        public Task<List<ChatMessageResponseData>> GetHistoryAsync(long chatId);

        public Task SendHistoryAsync(long chatId,
            string sender,
            string receiver,
            IHubCallerClients clients);
    }
}
