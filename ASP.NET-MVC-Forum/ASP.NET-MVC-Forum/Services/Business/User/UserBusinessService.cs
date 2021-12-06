namespace ASP.NET_MVC_Forum.Services.Business.User
{
    using ASP.NET_MVC_Forum.Areas.Admin.Models.User;
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Services.User;
    using AutoMapper;
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class UserBusinessService : IUserBusinessService
    {
        private readonly IUserDataService userDataService;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IMapper mapper;

        public UserBusinessService(IUserDataService userService, UserManager<IdentityUser> userManager, IMapper mapper)
        {
            this.userDataService = userService;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<List<UserViewModel>> GenerateUserViewModelAsync()
        {
            var allUsers = userDataService.GetAll(UserQueryFilter.WithIdentityUser, UserQueryFilter.AsNoTracking);

            var vm = mapper
                .Map<List<UserViewModel>>(allUsers)
                .ToList();

            return await ReturnUsersWithRoles(vm);
        }

        public async Task BanAsync(int userId)
        {
            var currentDateAndTime = DateTime.UtcNow;

            var user = await userDataService.GetByIdAsync(userId, UserQueryFilter.WithIdentityUser);

            user.IsBanned = true;

            user.IdentityUser.LockoutEnd = currentDateAndTime.AddYears(100);

            user.IdentityUser.LockoutEnabled = true;

            user.ModifiedOn = currentDateAndTime;

            await userManager
                .UpdateSecurityStampAsync(user.IdentityUser);

            await userDataService.UpdateAsync(user);
        }

        /// <summary>
        /// Ubans the user by setting it's IsBanned property to false and marking his linked IdentityUser's LockoutEnabled property to "false"
        /// </summary>
        /// <param name="userId">BaseUser's Id</param>
        public async Task UnbanAsync(int userId)
        {
            var user = await userDataService.GetByIdAsync(userId, UserQueryFilter.WithIdentityUser);

            user.IsBanned = false;

            user.IdentityUser.LockoutEnabled = false;

            user.ModifiedOn = DateTime.UtcNow;

            await userDataService.UpdateAsync(user);
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
