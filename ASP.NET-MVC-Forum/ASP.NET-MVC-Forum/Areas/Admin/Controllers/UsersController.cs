namespace ASP.NET_MVC_Forum.Areas.Admin.Controllers
{
    using ASP.NET_MVC_Forum.Areas.Admin.Models.User;
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Services.User;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
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
        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public UsersController(IUserService userService, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.userService = userService;
            this.mapper = mapper;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var allUsers = userService.GetAll(UserQueryFilter.WithIdentityUser);

            var vm = mapper
                .Map<List<UserViewModel>>(allUsers)
                .ToList();

            var vmWithRoles = await ReturnUsersWithRoles(vm);

            return View(vmWithRoles);
        }

        public IActionResult Ban(int userId)
        {
            if (!userService.UserExists(userId))
            {
                this.RedirectToActionWithErrorMessage(UserDoesNotExist, "Users", "Index");
            }

            if (userService.IsBanned(userId))
            {
                this.RedirectToActionWithErrorMessage($"User with Id {userId} is already banned !", "Users", "Index");
            }

            userService.Ban(userId);

            TempData[SuccessMessage] = $"User with Id {userId} has been successfully banned indefinitely";

            return RedirectToAction("Index");
        }

        public IActionResult RemoveBan(int userId)
        {
            if (!userService.UserExists(userId))
            {
                return this.RedirectToActionWithErrorMessage(UserDoesNotExist,"Users","Index");
            }

            if (!userService.IsBanned(userId))
            {
                TempData["ErrorMessage"] = $"User with Id {userId} is not banned !";
                return RedirectToAction("Index");
            }

            userService.Unban(userId);

            return this.RedirectToActionWithSuccessMessage($"User with Id {userId} has been successfully unbanned", "Users", "Index");
        }

        public async Task<IActionResult> Promote(int userId)
        {
            if (!userService.UserExists(userId))
            {
                return this.RedirectToActionWithErrorMessage(UserDoesNotExist, "Users", "Index");
            }

            var identityUser = userService
                .GetUser(userId, UserQueryFilter.WithIdentityUser)
                .First()
                .IdentityUser;

            bool isUserModerator = await userManager
                .IsInRoleAsync(identityUser, ModeratorRoleName);

            if (isUserModerator)
            {
                return this.RedirectToActionWithErrorMessage($"{identityUser.UserName} is already in the {ModeratorRoleName} position !", "Users", "Index");
            }

            userService.Promote(identityUser);

            return this.RedirectToActionWithSuccessMessage($"{identityUser.UserName} has been successfully promoted to {ModeratorRoleName}", "Users", "Index");
        }

        public async Task<IActionResult> Demote(int userId)
        {
            if (!userService.UserExists(userId))
            {
                return this.RedirectToActionWithErrorMessage(UserDoesNotExist, "Users", "Index");
            }

            var identityUser = userService
                .GetUser(userId, UserQueryFilter.WithIdentityUser)
                .First()
                .IdentityUser;

            var userRoles = await userManager.GetRolesAsync(identityUser);
            int userRolesCount = userRoles.Count;

            if (userRolesCount == 0)
            {
                return this.RedirectToActionWithErrorMessage($"{identityUser.UserName} cannot be further demoted !", "Users", "Index");
            }

            userService.Demote(identityUser);

            return this.RedirectToActionWithSuccessMessage($"{identityUser.UserName} has been successfully demoted !", "Users", "Index");
        }

        private async Task<List<UserViewModel>> ReturnUsersWithRoles(List<UserViewModel> users)
        {
            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user.IdentityUser);

                user.Roles = roles.ToList();
            }

            return users;
        }
    }
}
