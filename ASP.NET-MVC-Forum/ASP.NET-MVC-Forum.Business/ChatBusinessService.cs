namespace ASP.NET_MVC_Forum.Business
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Enums;
    using ASP.NET_MVC_Forum.Domain.Models.Chat;

    using AutoMapper;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using System;
    using System.Threading.Tasks;

    public class ChatBusinessService : IChatBusinessService
    {
        private readonly IMapper mapper;
        private readonly IUserRepository userRepo;
        private readonly IChatRepository chatRepo;

        public ChatBusinessService(
            IMapper mapper,
            IUserRepository userRepo,
            IChatRepository chatRepo)
        {
            this.mapper = mapper;
            this.userRepo = userRepo;
            this.chatRepo = chatRepo;
        }

        public async Task<ChatSelectUserViewModel> GenerateChatSelectUserViewModel(
            string recipientUsername,
            string currentIdentityUserId, 
            string currentIdentityUserUsername)
        {
            var identityUser = userRepo.GetByUsername(recipientUsername);

            var vm = await mapper
                .ProjectTo<ChatSelectUserViewModel>(identityUser)
                .FirstAsync();

            vm.SenderUsername = currentIdentityUserUsername;

            vm.SenderIdentityUserId = currentIdentityUserId;

            return vm;
        }

        public async Task<T> GenerateChatConversationViewModel<T>(string senderIdentityUserId, string recipientIdentityUserId, string senderUsername)
        {
            if (!await chatRepo.ChatExistsAsync(senderIdentityUserId, recipientIdentityUserId))
            {
                await chatRepo.CreateChatAsync(senderIdentityUserId, recipientIdentityUserId);
            }

            var chatId = await chatRepo.GetChatIdAsync(senderIdentityUserId, recipientIdentityUserId);

            return (T)Activator.CreateInstance(typeof(T), chatId, senderIdentityUserId, recipientIdentityUserId, senderUsername);
        }
    }
}
