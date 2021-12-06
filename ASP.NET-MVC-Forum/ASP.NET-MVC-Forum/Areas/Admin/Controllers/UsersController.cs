namespace ASP.NET_MVC_Forum.Areas.Admin.Controllers
{
    using ASP.NET_MVC_Forum.Services.Business.User;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using static ASP.NET_MVC_Forum.Data.Constants.ClientMessage.Error;
    using static ASP.NET_MVC_Forum.Data.Constants.ClientMessage.MessageType;
    using static ASP.NET_MVC_Forum.Data.Constants.ClientMessage.Success;
    using static ASP.NET_MVC_Forum.Data.Constants.RoleConstants;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ControllerExtensions;

    [Area("Admin")]
    [Authorize(Roles = AdminRoleName)]
    public class UsersController : Controller
    {
        private readonly IUserBusinessService userBusinessService;
        private readonly UserManager<IdentityUser> userManager;

        public UsersController(IUserBusinessService userBusinessService, UserManager<IdentityUser> userManager)
        {
            this.userBusinessService = userBusinessService;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            return View(await userBusinessService.GenerateUserViewModelAsync());
        }

        public async Task<IActionResult> Ban(int userId)
        {
            if (!await userBusinessService.UserExistsAsync(userId))
            {
                return this.RedirectToActionWithErrorMessage(UserDoesNotExist, "Users", "Index");
            }

            if (await userBusinessService.IsBannedAsync(userId))
            {
                return this.RedirectToActionWithErrorMessage(UserIsAlreadyBanned, "Users", "Index");
            }

            await userBusinessService.BanAsync(userId);

            TempData[SuccessMessage] = $"User with Id {userId} has been successfully banned indefinitely";

            return this.RedirectToActionWithSuccessMessage(UserSucessfullyBanned,"Users","Index");
        }

        public async Task<IActionResult> RemoveBan(int userId)
        {
            if (!await userBusinessService.UserExistsAsync(userId))
            {
                return this.RedirectToActionWithErrorMessage(UserDoesNotExist, "Users", "Index");
            }

            if (!await userBusinessService.IsBannedAsync(userId))
            {
                TempData[ErrorMessage] = $"User with Id {userId} is not banned !";
                return RedirectToAction("Index");
            }

            await userBusinessService.UnbanAsync(userId);

            return this.RedirectToActionWithSuccessMessage(UserSucessfullyUnBanned, "Users", "Index");
        }

        public async Task<IActionResult> Promote(int userId)
        {
            if (!await userBusinessService.UserExistsAsync(userId))
            {
                return this.RedirectToActionWithErrorMessage(UserDoesNotExist, "Users", "Index");
            }

            var identityUser = await userBusinessService.GetIdentityUser(userId);

            bool isUserModerator = await userManager
                .IsInRoleAsync(identityUser, ModeratorRoleName);

            if (isUserModerator)
            {
                return this.RedirectToActionWithErrorMessage(UserIsAlreadyAModerator, "Users", "Index");
            }

            await userBusinessService.PromoteAsync(userId);

            return this.RedirectToActionWithSuccessMessage(UserSuccessfullyPromoted, "Users", "Index");
        }

        public async Task<IActionResult> Demote(int userId)
        {
            if (!await userBusinessService.UserExistsAsync(userId))
            {
                return this.RedirectToActionWithErrorMessage(UserDoesNotExist, "Users", "Index");
            }

            var userRoles = await userBusinessService.GetUserRolesAsync(userId);

            if (userRoles.Count == 0)
            {
                return this.RedirectToActionWithErrorMessage(CannotFurtherDemote, "Users", "Index");
            }

            await userBusinessService.DemoteAsync(userId);

            return this.RedirectToActionWithSuccessMessage(UserSuccessfullyDemoted, "Users", "Index");
        }
    }
}
