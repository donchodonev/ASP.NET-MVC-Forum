namespace ASP.NET_MVC_Forum.Web.Areas.Admin.Controllers
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.ClientMessage;
    using static ASP.NET_MVC_Forum.Domain.Constants.RoleConstants;
    using static ASP.NET_MVC_Forum.Web.Extensions.ControllerExtensions;

    [Area("Admin")]
    [Authorize(Roles = AdminRoleName)]
    public class UsersController : Controller
    {
        private readonly IUserBusinessService userService;
        private readonly IUserRepository userRepo;

        public UsersController(
            IUserBusinessService userService,
            UserManager<ExtendedIdentityUser> userManager,
            IUserRepository userRepo)
        {
            this.userService = userService;
            this.userRepo = userRepo;
        }
        public async Task<IActionResult> Index()
        {
            return View(await userService.GenerateUserViewModelAsync());
        }

        public async Task<IActionResult> Ban(string userId)
        {
            if (!await userService.ExistsAsync(userId))
            {
                return this.RedirectToActionWithErrorMessage(Error.UserDoesNotExist, "Users", "Index");
            }

            if (await userService.IsBannedAsync(userId))
            {
                return this.RedirectToActionWithErrorMessage(Error.UserIsAlreadyBanned, "Users", "Index");
            }

            await userService.BanAsync(userId);

            TempData[MessageType.SuccessMessage] = $"User with Id {userId} has been successfully banned indefinitely";

            return this.RedirectToActionWithSuccessMessage(Success.UserSucessfullyBanned,"Users","Index");
        }

        public async Task<IActionResult> RemoveBan(string userId)
        {
            if (!await userService.ExistsAsync(userId))
            {
                return this.RedirectToActionWithErrorMessage(Error.UserDoesNotExist, "Users", "Index");
            }

            if (!await userService.IsBannedAsync(userId))
            {
                TempData[MessageType.ErrorMessage] = $"User with Id {userId} is not banned !";
                return RedirectToAction("Index");
            }

            await userService.UnbanAsync(userId);

            return this.RedirectToActionWithSuccessMessage(Success.UserSucessfullyUnBanned, "Users", "Index");
        }

        public async Task<IActionResult> Promote(string userId)
        {
            if (!await userService.ExistsAsync(userId))
            {
                return this.RedirectToActionWithErrorMessage(Error.UserDoesNotExist, "Users", "Index");
            }

            var identityUser = await userRepo.GetByIdAsync(userId);

            bool isUserModerator = await userService.IsUserInRoleAsync(identityUser, ModeratorRoleName);

            if (isUserModerator)
            {
                return this.RedirectToActionWithErrorMessage(Error.UserIsAlreadyAModerator, "Users", "Index");
            }

            await userService.PromoteAsync(userId);

            return this.RedirectToActionWithSuccessMessage(Success.UserSuccessfullyPromoted, "Users", "Index");
        }

        public async Task<IActionResult> Demote(string userId)
        {
            if (!await userService.ExistsAsync(userId))
            {
                return this.RedirectToActionWithErrorMessage(Error.UserDoesNotExist, "Users", "Index");
            }

            var userRoles = await userService.GetUserRolesAsync(userId);

            if (userRoles.Count == 0)
            {
                return this.RedirectToActionWithErrorMessage(Error.CannotFurtherDemote, "Users", "Index");
            }

            await userService.DemoteAsync(userId);

            return this.RedirectToActionWithSuccessMessage(Success.UserSuccessfullyDemoted, "Users", "Index");
        }
    }
}
