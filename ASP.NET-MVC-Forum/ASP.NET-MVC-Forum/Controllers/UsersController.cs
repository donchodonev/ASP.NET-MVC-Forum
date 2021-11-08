namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Models.Post;
    using ASP.NET_MVC_Forum.Services.Post;
    using ASP.NET_MVC_Forum.Services.User;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using System.Threading.Tasks;
    using static ASP.NET_MVC_Forum.Data.DataConstants.AllowedImageExtensions;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ClaimsPrincipalExtensions;

    [Authorize]
    public class UsersController : Controller
    {
        private readonly IPostService postService;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public UsersController(IPostService postService, IUserService userService, IMapper mapper)
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

        public IActionResult UploadAvatar(IFormFile file)
        {
            string fileExtension = userService.GetImageExtension(file);
            string identityUserId = this.User.Id();

            if (fileExtension == null)
            {
                string[] allowedFileExtensions = new string[5] { JPG, JPEG, PNG, WEBP, BMP };
                TempData["Message"] = $"The allowed image file formats are {string.Join(' ', allowedFileExtensions)}";
                return LocalRedirect("/Identity/Account/Manage#message");
            }

            userService.AvatarUpdate(identityUserId, file);

            TempData["Message"] = $"Your image has been successfully uploaded";

            return LocalRedirect("/Identity/Account/Manage#message");
        }
    }
}
