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
                .GenerateChatConversationViewModel(
                senderIdentityUserId,
                recipientIdentityUserId, 
                senderUsername);

            return View(vm);
        }

        public async Task<IActionResult> SelectUser(SelectUserInputModel userInput)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var vm = await chatService.GenerateChatSelectUserViewModel(userInput.Username, this.User.Id(), this.User.Identity.Name);

            return View(vm);
        }
    }
}