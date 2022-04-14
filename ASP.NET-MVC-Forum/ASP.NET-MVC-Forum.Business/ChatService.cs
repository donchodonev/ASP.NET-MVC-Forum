namespace ASP.NET_MVC_Forum.Business
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Models.Chat;
    using ASP.NET_MVC_Forum.Validation.Contracts;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;

    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ChatService : IChatService
    {
        private readonly IMapper mapper;
        private readonly IUserRepository userRepo;
        private readonly IChatRepository chatRepo;
        private readonly IUserValidationService userValidationService;
        private readonly IChatValidationService chatValidationService;

        public ChatService(
            IMapper mapper,
            IUserRepository userRepo,
            IChatRepository chatRepo,
            IUserValidationService userValidationService,
            IChatValidationService chatValidationService)
        {
            this.mapper = mapper;
            this.userRepo = userRepo;
            this.chatRepo = chatRepo;
            this.userValidationService = userValidationService;
            this.chatValidationService = chatValidationService;
        }

        public async Task<ChatSelectUserViewModel> GenerateChatSelectUserViewModel(
            string recipientUsername,
            string currentIdentityUserId,
            string currentIdentityUserUsername)
        {
            await userValidationService.ValidateUserExistsByUsernameAsync(currentIdentityUserUsername);

            await userValidationService.ValidateUserExistsByUsernameAsync(recipientUsername);

            var user = userRepo.GetByUsername(recipientUsername);

            var vm = await mapper
                .ProjectTo<ChatSelectUserViewModel>(user)
                .FirstAsync();

            vm.SenderUsername = currentIdentityUserUsername;

            vm.SenderIdentityUserId = currentIdentityUserId;

            return vm;
        }

        public async Task<ChatConversationViewModel> GenerateChatConversationViewModel(
            string senderId,
            string recipientId,
            string senderUsername)
        {
            await userValidationService.ValidateUserExistsByIdAsync(senderId);

            await userValidationService.ValidateUserExistsByIdAsync(recipientId);

            await chatRepo.EnsureChatExistsAsync(senderId, recipientId);

            var chatId = await chatRepo.GetChatIdAsync(senderId, recipientId);

            return new ChatConversationViewModel(chatId, senderId, recipientId, senderUsername);
        }

        public async Task SendMessageToClientsAsync(
            string senderId,
            string receiverId,
            string message,
            long chatId,
            string senderUsername,
            IHubCallerClients clients)
        {
            await userValidationService.ValidateUserExistsByIdAsync(senderId);

            await userValidationService.ValidateUserExistsByIdAsync(receiverId);

            var persistedMessage = await chatRepo.AddMessageAsync(chatId, message, senderUsername);

            var time = persistedMessage
                .CreatedOn
                .AddHours(2) // FOR GMT+2
                .ToString("HH:mm:ss");

            var response = new ChatMessageResponseData(senderUsername, time, persistedMessage.Text);

            await clients.Group(senderId + receiverId).SendAsync("ReceiveMessage", response);

            await clients.Group(receiverId + senderId).SendAsync("ReceiveMessage", response);
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
            await chatValidationService.ValidateChatExistsAsync(chatId);

            var messages = await GetHistoryAsync(chatId);

            await clients.Group(sender + receiver).SendAsync("ReceiveHistory", messages);
        }
    }
}
