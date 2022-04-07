namespace ASP.NET_MVC_Forum.Web.Controllers
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Infrastructure.Extensions;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using System.Threading.Tasks;

    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            string identityUserId = this.User.Id();

            await userService.AvatarUpdateAsync(identityUserId, file);

            TempData["Message"] = $"Your image has been successfully uploaded";

            return LocalRedirect("/Identity/Account/Manage#message");
        }
    }
}
