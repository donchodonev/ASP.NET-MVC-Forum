namespace ASP.NET_MVC_Forum.Business
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Business.Contracts.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Models.Chat;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ChatService : IChatService
    {
        private readonly IMapper mapper;
        private readonly IUserRepository userRepo;
        private readonly IChatRepository chatRepo;
        private readonly IUserValidationService userValidationService;

        public ChatService(
            IMapper mapper,
            IUserRepository userRepo,
            IChatRepository chatRepo,
            IUserValidationService userValidationService)
        {
            this.mapper = mapper;
            this.userRepo = userRepo;
            this.chatRepo = chatRepo;
            this.userValidationService = userValidationService;
        }

        public async Task<ChatSelectUserViewModel> GenerateChatSelectUserViewModel(
            string recipientUsername,
            string currentIdentityUserId,
            string currentIdentityUserUsername)
        {
            await userValidationService.ValidateUserExistsAsync(recipientUsername);

            var user = userRepo.GetByUsername(recipientUsername);

            var vm = await mapper
                .ProjectTo<ChatSelectUserViewModel>(user)
                .FirstAsync();

            vm.SenderUsername = currentIdentityUserUsername;

            vm.SenderIdentityUserId = currentIdentityUserId;

            return vm;
        }

        public async Task<T> GenerateChatConversationViewModel<T>(string senderIdentityUserId, string recipientIdentityUserId, string senderUsername)
        {
            await chatRepo.CreateChatAsync(senderIdentityUserId, recipientIdentityUserId);

            var chatId = await chatRepo.GetChatIdAsync(senderIdentityUserId, recipientIdentityUserId);

            return (T)Activator.CreateInstance(typeof(T), chatId, senderIdentityUserId, recipientIdentityUserId, senderUsername);
        }

        public async Task SendMessageToClientsAsync(
            string senderIdentityId,
            string receiverIdentityId,
            string message,
            long chatId,
            string senderUsername,
            IHubCallerClients clients)
        {
            var persistedMessage = await chatRepo.AddMessageAsync(chatId, message, senderUsername);

            var time = persistedMessage
                .CreatedOn
                .AddHours(2) // FOR GMT+2
                .ToString("HH:mm:ss");

            var response = new ChatMessageResponseData(senderUsername, time, persistedMessage.Text);

            await clients.Group(senderIdentityId + receiverIdentityId).SendAsync("ReceiveMessage", response);

            await clients.Group(receiverIdentityId + senderIdentityId).SendAsync("ReceiveMessage", response);
        }

        public Task<List<ChatMessageResponseData>> GetHistoryAsync(long chatId)
        {
            return chatRepo
                    .GetLastMessagesAsNoTracking(chatId)
                    .ProjectTo<ChatMessageResponseData>(mapper.ConfigurationProvider)
                    .ToListAsync();
        }

        public async Task SendHistoryAsync(long chatId,
            string sender,
            string receiver,
            IHubCallerClients clients)
        {
            var messages = await GetHistoryAsync(chatId);

            await clients.Group(sender + receiver).SendAsync("ReceiveHistory", messages);
        }
    }
}
