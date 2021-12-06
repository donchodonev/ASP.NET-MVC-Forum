namespace ASP.NET_MVC_Forum.Areas.Admin.Controllers
{
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Services.Business.User;
    using ASP.NET_MVC_Forum.Services.User;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Data.Constants.ClientMessage.Error;
    using static ASP.NET_MVC_Forum.Data.Constants.ClientMessage.MessageType;
    using static ASP.NET_MVC_Forum.Data.Constants.RoleConstants;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ControllerExtensions;

    [Area("Admin")]
    [Authorize(Roles = AdminRoleName)]
    public class UsersController : Controller
    {
        private readonly IUserDataService userDataService;
        private readonly IUserBusinessService userBusinessService;
        private readonly UserManager<IdentityUser> userManager;

        public UsersController(IUserDataService userService, IUserBusinessService userBusinessService, UserManager<IdentityUser> userManager)
        {
            this.userDataService = userService;
            this.userBusinessService = userBusinessService;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            return View(await userBusinessService.GenerateUserViewModelAsync());
        }

        public async Task<IActionResult> Ban(int userId)
        {
            if (!await userDataService.UserExistsAsync(userId))
            {
                return this.RedirectToActionWithErrorMessage(UserDoesNotExist, "Users", "Index");
            }

            if (userDataService.IsBanned(userId))
            {
                return this.RedirectToActionWithErrorMessage($"User with Id {userId} is already banned !", "Users", "Index");
            }

            await userBusinessService.BanAsync(userId);

            TempData[SuccessMessage] = $"User with Id {userId} has been successfully banned indefinitely";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RemoveBan(int userId)
        {
            if (!await userDataService.UserExistsAsync(userId))
            {
                return this.RedirectToActionWithErrorMessage(UserDoesNotExist, "Users", "Index");
            }

            if (!userDataService.IsBanned(userId))
            {
                TempData[ErrorMessage] = $"User with Id {userId} is not banned !";
                return RedirectToAction("Index");
            }

            await userBusinessService.UnbanAsync(userId);

            return this.RedirectToActionWithSuccessMessage($"User with Id {userId} has been successfully unbanned", "Users", "Index");
        }

        public async Task<IActionResult> Promote(int userId)
        {
            if (!await userDataService.UserExistsAsync(userId))
            {
                return this.RedirectToActionWithErrorMessage(UserDoesNotExist, "Users", "Index");
            }

            var identityUser = userDataService
                .GetUser(userId, UserQueryFilter.WithIdentityUser)
                .First()
                .IdentityUser;

            bool isUserModerator = await userManager
                .IsInRoleAsync(identityUser, ModeratorRoleName);

            if (isUserModerator)
            {
                return this.RedirectToActionWithErrorMessage($"{identityUser.UserName} is already in the {ModeratorRoleName} position !", "Users", "Index");
            }

            await userDataService.PromoteAsync(identityUser);

            return this.RedirectToActionWithSuccessMessage($"{identityUser.UserName} has been successfully promoted to {ModeratorRoleName}", "Users", "Index");
        }

        public async Task<IActionResult> Demote(int userId)
        {
            if (!await userDataService.UserExistsAsync(userId))
            {
                return this.RedirectToActionWithErrorMessage(UserDoesNotExist, "Users", "Index");
            }

            var identityUser = userDataService
                .GetUser(userId, UserQueryFilter.WithIdentityUser)
                .First()
                .IdentityUser;

            var userRoles = await userManager.GetRolesAsync(identityUser);
            int userRolesCount = userRoles.Count;

            if (userRolesCount == 0)
            {
                return this.RedirectToActionWithErrorMessage($"{identityUser.UserName} cannot be further demoted !", "Users", "Index");
            }

            await userDataService.DemoteAsync(identityUser);

            return this.RedirectToActionWithSuccessMessage($"{identityUser.UserName} has been successfully demoted !", "Users", "Index");
        }


    }
}
