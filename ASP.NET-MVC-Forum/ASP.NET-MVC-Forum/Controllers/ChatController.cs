namespace ASP.NET_MVC_Forum.Web.Controllers
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Domain.Models.Chat;
    using ASP.NET_MVC_Forum.Infrastructure.Extensions;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using System.Threading.Tasks;

    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService chatService;
        private readonly IUserService userService;

        public ChatController(
            IChatService chatService,
            IUserService userService)
        {
            this.chatService = chatService;
            this.userService = userService;
        }
        public async Task<IActionResult> ChatConversation(string senderIdentityUserId, string recipientIdentityUserId, string senderUsername)
        {
            var vm = await chatService
                .GenerateChatConversationViewModel<ChatConversationViewModel>
                (senderIdentityUserId, recipientIdentityUserId, senderUsername);

            return View(vm);
        }

        public async Task<IActionResult> SelectUser(string username)
        {
            var vm = await chatService.GenerateChatSelectUserViewModel(username,this.User.Id(),this.User.Identity.Name);

            return View(vm);
        }
    }
}