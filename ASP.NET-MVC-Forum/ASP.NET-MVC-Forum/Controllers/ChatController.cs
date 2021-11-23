namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Infrastructure.Extensions;
    using ASP.NET_MVC_Forum.Models.Chat;
    using ASP.NET_MVC_Forum.Services.Chat;
    using ASP.NET_MVC_Forum.Services.User;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using System.Threading.Tasks;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ControllerExtensions;
    [Authorize]

    public class ChatController : Controller
    {
        private readonly IUserService userService;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IMapper mapper;
        private readonly IChatService chatService;

        public ChatController(IUserService userService, UserManager<IdentityUser> userManager, IMapper mapper, IChatService chatService)
        {
            this.userService = userService;
            this.userManager = userManager;
            this.mapper = mapper;
            this.chatService = chatService;
        }
        public async Task<IActionResult> ChatConversation(string senderIdentityUserId, string recipientIdentityUserId, string senderUsername)
        {
            var chatId = await GetChatId(senderIdentityUserId, recipientIdentityUserId);

            var vm = new ChatConversationViewModel(
                chatId,
                senderIdentityUserId,
                recipientIdentityUserId,
                senderUsername);

            return View(vm);
        }

        public async Task<IActionResult> SelectUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return View();
            }
            else if (username.Length < 4)
            {
                return this.ViewWithErrorMessage($"Username must be at least 4 symbols long");
            }

            var identityUser = await userManager.FindByNameAsync(username);

            if (identityUser == null)
            {
                return this.ViewWithErrorMessage($"No users found with the username \"{username}\"");
            }

            var vm = GetViewModelWithData(identityUser);

            return View(vm);
        }
        private async Task<long> GetChatId(string sender, string receiver)
        {
            if (!await chatService.ChatExistsAsync(sender, receiver))
            {
                return await chatService.CreateChatAsync(sender, receiver);
            }

            return await chatService.GetChatIdAsync(sender, receiver);
        }


        private ChatSelectUserViewModel GetViewModelWithData(IdentityUser identityUser)
        {
            var vm = mapper
                .ProjectTo<ChatSelectUserViewModel>(userService.GetUser(identityUser.Id))
                .First();

            vm.RecipientUsername = identityUser.UserName;
            vm.RecipientIdentityUserId = identityUser.Id;
            vm.SenderUsername = this.User.Identity.Name;
            vm.SenderIdentityUserId = this.User.Id();

            return vm;
        }
    }
}