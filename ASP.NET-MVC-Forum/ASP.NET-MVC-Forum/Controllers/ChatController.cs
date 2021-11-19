namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Models.Chat;
    using ASP.NET_MVC_Forum.Services.User;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using System.Threading.Tasks;

    [Authorize]

    public class ChatController : Controller
    {
        private readonly IUserService userService;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IMapper mapper;

        public ChatController(IUserService userService, UserManager<IdentityUser> userManager,IMapper mapper)
        {
            this.userService = userService;
            this.userManager = userManager;
            this.mapper = mapper;
        }
        public IActionResult ChatConversation()
        {
            return View();
        }

        public async Task<IActionResult> SelectUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return View();
            }
            else if (username.Length < 4)
            {
                TempData["ErrorMessage"] = $"Username must be at least 4 symbols long";
                return View();
            }

            var identityUser = await userManager.FindByNameAsync(username);

            if (identityUser == null)
            {
                TempData["ErrorMessage"] = $"No users found with the username \"{username}\"";
                return View();
            }

            var vm = mapper
                .ProjectTo<ChatSelectUserViewModel>(userService.GetUser(identityUser.Id))
                .First();

            vm.Username = identityUser.UserName;
            vm.IdentityUserId = identityUser.Id;

            return View(vm);
        }
    }
}
