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
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ClaimsPrincipalExtensions;

    public class UsersController : Controller
    {
        private readonly IPostService postService;
        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public UsersController(IPostService postService, IUserService userService, IMapper mapper, IConfiguration configuration)
        {
            this.postService = postService;
            this.userService = userService;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        [Authorize]
        public async Task<IActionResult> UserPosts()
        {
            var userId = await userService.GetBaseUserIdAsync(this.User.Id());
            var vm = mapper.ProjectTo<PostPreviewViewModel>(await postService.GetByUserIdAsync(userId, withUserIncluded: true, withIdentityUserIncluded: true));

            return View("_PostsPreviewPartial", vm.ToList());
        }

        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            string[] allowedFileExtensions = new string[5] { ".jpg", ".jpeg", ".png", ".webp",".bmp" };

            string fileExtension = null;

            foreach (var currentFileExtension in allowedFileExtensions)
            {
                if (file.FileName.EndsWith(currentFileExtension))
                {
                    fileExtension = currentFileExtension;
                    break;
                }
            }

            if (fileExtension == null)
            {
                TempData["Message"] = $"The allowed image file formats are {string.Join(' ',allowedFileExtensions)}";
                return LocalRedirect("/Identity/Account/Manage#message");
            }

            var fileName = $"{this.User.Identity.Name}-avatar{fileExtension}";
            var filePath = configuration.GetSection("FileUploadPath")["DefaultPath"];
            var fullPath = Path.Combine(filePath, fileName);

            string test = configuration.GetSection("FileUploadPath")["DefaultPath"];

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
