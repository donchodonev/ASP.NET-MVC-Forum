﻿namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Models.Post;
    using ASP.NET_MVC_Forum.Services.Post;
    using ASP.NET_MVC_Forum.Services.User;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.FileProviders;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ClaimsPrincipalExtensions;

    [Authorize]
    public class UsersController : Controller
    {
        private readonly IPostService postService;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public UsersController(IPostService postService, IUserService userService, IMapper mapper, IConfiguration configuration)
        {
            this.postService = postService;
            this.userService = userService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> UserPosts()
        {
            var userId = await userService.GetBaseUserIdAsync(this.User.Id());
            var vm = mapper.ProjectTo<PostPreviewViewModel>(await postService.GetByUserIdAsync(userId, withUserIncluded: true, withIdentityUserIncluded: true));

            return View("_PostsPreviewPartial", vm.ToList());
        }
    }
}
