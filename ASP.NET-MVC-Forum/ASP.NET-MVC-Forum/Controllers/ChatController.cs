namespace ASP.NET_MVC_Forum.Web.Controllers
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Domain.Models.Chat;
    using ASP.NET_MVC_Forum.Infrastructure.Extensions;
    using ASP.NET_MVC_Forum.Web.Extensions;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.ClientMessage;
    using static ASP.NET_MVC_Forum.Domain.Constants.DataConstants;

    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService chatService;

        public ChatController(
            IChatService chatService)
        {
            this.chatService = chatService;
        }
        public async Task<IActionResult> ChatConversation(
            string senderIdentityUserId,
            string recipientIdentityUserId,
            string senderUsername)
        {
            var vm = await chatService
                .GenerateChatConversationViewModel<ChatConversationViewModel>
                (senderIdentityUserId, recipientIdentityUserId, senderUsername);

            return View(vm);
        }

        public async Task<IActionResult> SelectUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return View();
            }
            else if (username.Length < UserConstants.USERNAME_MIN_LENGTH)
            {
                return this.ViewWithErrorMessage(Error.USERNAME_TOO_SHORT);
            }

            var vm = await chatService.GenerateChatSelectUserViewModel(username, this.User.Id(), this.User.Identity.Name);

            return View(vm);
        }
    }
}