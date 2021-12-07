namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Infrastructure.Extensions;
    using ASP.NET_MVC_Forum.Models.Chat;
    using ASP.NET_MVC_Forum.Services.Business.Chat;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using static ASP.NET_MVC_Forum.Data.Constants.ClientMessage;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ControllerExtensions;
    using static ASP.NET_MVC_Forum.Data.Constants.DataConstants;

    [Authorize]
    public class ChatController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IChatBusinessService chatBusinessService;

        public ChatController(UserManager<IdentityUser> userManager, IChatBusinessService chatBusinessService)
        {
            this.userManager = userManager;
            this.chatBusinessService = chatBusinessService;
        }
        public async Task<IActionResult> ChatConversation(string senderIdentityUserId, string recipientIdentityUserId, string senderUsername)
        {
            var vm = await chatBusinessService
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
            else if (username.Length < UserConstants.UsernameMinLength)
            {
                return this.ViewWithErrorMessage(Error.UsernameTooShort);
            }

            var identityUser = await userManager.FindByNameAsync(username);

            if (identityUser == null)
            {
                return this.ViewWithErrorMessage(Error.UserDoesNotExist);
            }

            var vm = await chatBusinessService.GenerateChatSelectUserViewModel(username,this.User.Id(),this.User.Identity.Name);

            return View(vm);
        }
    }
}