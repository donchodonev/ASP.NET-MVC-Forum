namespace ASP.NET_MVC_Forum.Web.Areas.Admin.Controllers
{
    using ASP.NET_MVC_Forum.Business.Contracts;
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
                return this.RedirectToActionWithErrorMessage(Error.UserDoesNotExist, "Users", "Index");
            }

            if (await userBusinessService.IsBannedAsync(userId))
            {
                return this.RedirectToActionWithErrorMessage(Error.UserIsAlreadyBanned, "Users", "Index");
            }

            await userBusinessService.BanAsync(userId);

            TempData[MessageType.SuccessMessage] = $"User with Id {userId} has been successfully banned indefinitely";

            return this.RedirectToActionWithSuccessMessage(Success.UserSucessfullyBanned,"Users","Index");
        }

        public async Task<IActionResult> RemoveBan(int userId)
        {
            if (!await userBusinessService.UserExistsAsync(userId))
            {
                return this.RedirectToActionWithErrorMessage(Error.UserDoesNotExist, "Users", "Index");
            }

            if (!await userBusinessService.IsBannedAsync(userId))
            {
                TempData[MessageType.ErrorMessage] = $"User with Id {userId} is not banned !";
                return RedirectToAction("Index");
            }

            await userBusinessService.UnbanAsync(userId);

            return this.RedirectToActionWithSuccessMessage(Success.UserSucessfullyUnBanned, "Users", "Index");
        }

        public async Task<IActionResult> Promote(int userId)
        {
            if (!await userBusinessService.UserExistsAsync(userId))
            {
                return this.RedirectToActionWithErrorMessage(Error.UserDoesNotExist, "Users", "Index");
            }

            var identityUser = await userBusinessService.GetIdentityUser(userId);

            bool isUserModerator = await userManager
                .IsInRoleAsync(identityUser, ModeratorRoleName);

            if (isUserModerator)
            {
                return this.RedirectToActionWithErrorMessage(Error.UserIsAlreadyAModerator, "Users", "Index");
            }

            await userBusinessService.PromoteAsync(userId);

            return this.RedirectToActionWithSuccessMessage(Success.UserSuccessfullyPromoted, "Users", "Index");
        }

        public async Task<IActionResult> Demote(int userId)
        {
            if (!await userBusinessService.UserExistsAsync(userId))
            {
                return this.RedirectToActionWithErrorMessage(Error.UserDoesNotExist, "Users", "Index");
            }

            var userRoles = await userBusinessService.GetUserRolesAsync(userId);

            if (userRoles.Count == 0)
            {
                return this.RedirectToActionWithErrorMessage(Error.CannotFurtherDemote, "Users", "Index");
            }

            await userBusinessService.DemoteAsync(userId);

            return this.RedirectToActionWithSuccessMessage(Success.UserSuccessfullyDemoted, "Users", "Index");
        }
    }
}
