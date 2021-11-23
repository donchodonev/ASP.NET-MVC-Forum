﻿namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Services.Data.Post;
    using ASP.NET_MVC_Forum.Services.User;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ClaimsPrincipalExtensions;

    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserService userService;

        public UsersController(IPostDataService postService, IUserService userService, IMapper mapper)
        {
            this.userService = userService;
        }

        public IActionResult UploadAvatar(IFormFile file)
        {
            string identityUserId = this.User.Id();

            try
            {
                userService.AvatarUpdate(identityUserId, file);
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
