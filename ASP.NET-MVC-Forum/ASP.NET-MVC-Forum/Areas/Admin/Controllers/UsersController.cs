namespace ASP.NET_MVC_Forum.Areas.Admin.Controllers
{
    using ASP.NET_MVC_Forum.Areas.Admin.Models.User;
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
            var users = mapper
                .Map<List<UserViewModel>>(userService.GetAll(withIdentityIncluded: true))
                .ToList();

            return View(ReturnUsersWithRoles(users));
        }

        public IActionResult Ban(int userId)
        {
            EnsureUserExists(userId);
            EnsureIsNotBanned(userId);

            userService.Ban(userId);

            TempData["SuccessMessage"] = $"User with Id {userId} has been successfully banned indefinitely";

            return RedirectToAction("Index");
        }

        public IActionResult RemoveBan(int userId)
        {
            EnsureUserExists(userId);
            EnsureIsBanned(userId);

            userService.Unban(userId);

            TempData["SuccessMessage"] = $"User with Id {userId} has been successfully unbanned";

            return RedirectToAction("Index");
        }

        public IActionResult Promote(int userId)
        {



            return RedirectToAction("Index");
        }
        private void EnsureIsBanned(int userId)
        {
            if (!userService.IsBanned(userId))
            {
                TempData["ErrorMessage"] = $"User with Id {userId} is not banned !";
                RedirectToAction("Index");
            }
        }

        private void EnsureIsNotBanned(int userId)
        {
            if (userService.IsBanned(userId))
            {
                TempData["ErrorMessage"] = $"User with Id {userId} is already banned !";
                RedirectToAction("Index");
            }
        }

        private void EnsureUserExists(int userId)
        {
            if (!userService.UserExists(userId))
            {
                TempData["ErrorMessage"] = "User does NOT exist !";
                RedirectToAction("Index");
            }
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
