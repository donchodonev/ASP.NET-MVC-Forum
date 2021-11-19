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

    using static ASP.NET_MVC_Forum.Data.DataConstants.RoleConstants;

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
        public IActionResult Index()
        {
            var allUsers = userService.GetAll(UserQueryFilter.WithIdentityUser);

            var vm = mapper
                .Map<List<UserViewModel>>(allUsers)
                .ToList();

            var vmWithRoles = ReturnUsersWithRoles(vm);

            return View(vmWithRoles);
        }

        public IActionResult Ban(int userId)
        {
            if (!userService.UserExists(userId))
            {
                TempData["ErrorMessage"] = "User does NOT exist !";
                return RedirectToAction("Index");
            }


            if (userService.IsBanned(userId))
            {
                TempData["ErrorMessage"] = $"User with Id {userId} is already banned !";
                return RedirectToAction("Index");
            }

            userService.Ban(userId);

            TempData["SuccessMessage"] = $"User with Id {userId} has been successfully banned indefinitely";

            return RedirectToAction("Index");
        }

        public IActionResult RemoveBan(int userId)
        {
            if (!userService.UserExists(userId))
            {
                TempData["ErrorMessage"] = "User does NOT exist !";
                return RedirectToAction("Index");
            }

            if (!userService.IsBanned(userId))
            {
                TempData["ErrorMessage"] = $"User with Id {userId} is not banned !";
                return RedirectToAction("Index");
            }

            userService.Unban(userId);

            TempData["SuccessMessage"] = $"User with Id {userId} has been successfully unbanned";

            return RedirectToAction("Index");
        }

        public IActionResult Promote(int userId)
        {
            if (!userService.UserExists(userId))
            {
                TempData["ErrorMessage"] = "User does NOT exist !";
                return RedirectToAction("Index");
            }

            var identityUser = userService
                .GetUser(userId,UserQueryFilter.WithIdentityUser)
                .First()
                .IdentityUser;

            bool isUserModerator = userManager
                .IsInRoleAsync(identityUser, ModeratorRoleName)
                .GetAwaiter()
                .GetResult();

            if (isUserModerator)
            {
                TempData["ErrorMessage"] = $"{identityUser.UserName} is already in the {ModeratorRoleName} position !";
                return RedirectToAction("Index");
            }

            userService.Promote(identityUser);

            TempData["SuccessMessage"] = $"{identityUser.UserName} has been successfully promoted to {ModeratorRoleName}";

            return RedirectToAction("Index");
        }

        public IActionResult Demote(int userId)
        {
            if (!userService.UserExists(userId))
            {
                TempData["ErrorMessage"] = "User does NOT exist !";
                return RedirectToAction("Index");
            }

            var identityUser = userService
                .GetUser(userId,UserQueryFilter.WithIdentityUser)
                .First()
                .IdentityUser;

            int userRolesCount = userManager.GetRolesAsync(identityUser).GetAwaiter().GetResult().Count;

            if (userRolesCount == 0)
            {
                TempData["ErrorMessage"] = $"{identityUser.UserName} cannot be further demoted !";
                return RedirectToAction("Index");
            }

            userService.Demote(identityUser);

            TempData["SuccessMessage"] = $"{identityUser.UserName} has been successfully demoted !";

            return RedirectToAction("Index");
        }

        private List<UserViewModel> ReturnUsersWithRoles(List<UserViewModel> users)
        {
            foreach (var user in users)
            {
                user.Roles = userManager
                    .GetRolesAsync(user.IdentityUser)
                    .GetAwaiter()
                    .GetResult()
                    .ToList();
            }

            return users;
        }
    }
}
