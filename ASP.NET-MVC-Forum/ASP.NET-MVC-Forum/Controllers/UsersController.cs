namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Services.Business.User;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ClaimsPrincipalExtensions;

    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserBusinessService userService;

        public UsersController(IUserBusinessService userService)
        {
            this.userService = userService;
        }

        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            string identityUserId = this.User.Id();

            try
            {
                await userService.AvatarUpdateAsync(identityUserId, file);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                TempData["Message"] = ex.ParamName;
                return LocalRedirect("/Identity/Account/Manage#message");
            }

            TempData["Message"] = $"Your image has been successfully uploaded";

            return LocalRedirect("/Identity/Account/Manage#message");
        }
    }
}
