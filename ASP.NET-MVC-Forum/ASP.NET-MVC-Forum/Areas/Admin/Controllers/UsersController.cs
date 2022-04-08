namespace ASP.NET_MVC_Forum.Web.Areas.Admin.Controllers
{
    using ASP.NET_MVC_Forum.Business.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.ClientMessage;
    using static ASP.NET_MVC_Forum.Domain.Constants.RoleConstants;
    using static ASP.NET_MVC_Forum.Web.Extensions.ControllerExtensions;

    [Area("Admin")]
    [Authorize(Roles = ADMIN_ROLE)]
    public class UsersController : Controller
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await userService.GenerateUserViewModelAsync());
        }

        public async Task<IActionResult> Ban(string userId)
        {
            await userService.BanAsync(userId);

            return this.RedirectToActionWithSuccessMessage(Success.USER_BANNED, "Users", "Index");
        }

        public async Task<IActionResult> RemoveBan(string userId)
        {
            await userService.UnbanAsync(userId);

            return this.RedirectToActionWithSuccessMessage(Success.USER_UNBANNED, "Users", "Index");
        }

        public async Task<IActionResult> Promote(string userId)
        {
            await userService.PromoteAsync(userId);

            return this.RedirectToActionWithSuccessMessage(Success.USER_PROMOTED, "Users", "Index");
        }

        public async Task<IActionResult> Demote(string userId)
        {
            await userService.DemoteAsync(userId);

            return this.RedirectToActionWithSuccessMessage(Success.USER_DEMOTED, "Users", "Index");
        }
    }
}
