using ASP.NET_MVC_Forum.Data.Enums;
using ASP.NET_MVC_Forum.Models.Chat;
using ASP.NET_MVC_Forum.Services.Data.Chat;
using ASP.NET_MVC_Forum.Services.Data.User;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ASP.NET_MVC_Forum.Services.Business.Chat
{
    public class ChatBusinessService : IChatBusinessService
    {
        private readonly IMapper mapper;
        private readonly IUserDataService data;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IChatDataService chatDataService;

        public ChatBusinessService(IMapper mapper, IUserDataService data, UserManager<IdentityUser> userManager, IChatDataService chatDataService)
        {
            this.mapper = mapper;
            this.data = data;
            this.userManager = userManager;
            this.chatDataService = chatDataService;
        }

        public async Task<ChatSelectUserViewModel> GenerateChatSelectUserViewModel(string recipientUsername, string currentIdentityUserId, string currentIdentityUserUsername)
        {
            var identityUser = await userManager.FindByNameAsync(recipientUsername);

            var vm = await mapper
                .ProjectTo<ChatSelectUserViewModel>(data.GetUser(identityUser.Id, UserQueryFilter.AsNoTracking, UserQueryFilter.WithIdentityUser))
                .FirstAsync();

            vm.SenderUsername = currentIdentityUserUsername;
            vm.SenderIdentityUserId = currentIdentityUserId;

            return vm;
        }

        public async Task<T> GenerateChatConversationViewModel<T>(string senderIdentityUserId, string recipientIdentityUserId, string senderUsername)
        {
            if (!await chatDataService.ChatExistsAsync(senderIdentityUserId, recipientIdentityUserId))
            {
                await chatDataService.CreateChatAsync(senderIdentityUserId, recipientIdentityUserId);
            }

            var chatId = await chatDataService.GetChatIdAsync(senderIdentityUserId, recipientIdentityUserId);

            return (T)Activator.CreateInstance(typeof(T), chatId, senderIdentityUserId, recipientIdentityUserId, senderUsername);
        }
    }
}
