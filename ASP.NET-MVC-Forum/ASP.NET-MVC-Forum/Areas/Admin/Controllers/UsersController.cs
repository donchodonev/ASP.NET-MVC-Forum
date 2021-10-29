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

    [Area("Admin")]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public UsersController(IUserService userService,IMapper mapper,UserManager<IdentityUser> userManager)
        {
            this.userService = userService;
            this.mapper = mapper;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            var users = mapper
                .Map<List<UserViewModel>>(userService.GetAll(withIdentityIncluded:true))
                .ToList();

            var usersWithRoles = GetUsersWithRoles(users);

            return View(usersWithRoles);
        }

        private List<UserViewModel> GetUsersWithRoles(List<UserViewModel> users)
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
