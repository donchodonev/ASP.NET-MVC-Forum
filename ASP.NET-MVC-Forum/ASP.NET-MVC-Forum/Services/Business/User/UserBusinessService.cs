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
        private readonly IUserDataService data;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IMapper mapper;

        public UserBusinessService(IUserDataService data, UserManager<IdentityUser> userManager, IMapper mapper)
        {
            this.data = data;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<List<UserViewModel>> GenerateUserViewModelAsync()
        {
            var allUsers = data.GetAll(UserQueryFilter.WithIdentityUser, UserQueryFilter.AsNoTracking);

            var vm = mapper
                .Map<List<UserViewModel>>(allUsers)
                .ToList();

            return await ReturnUsersWithRoles(vm);
        }

        public async Task BanAsync(int userId)
        {
            var currentDateAndTime = DateTime.UtcNow;

            var user = await data.GetByIdAsync(userId, UserQueryFilter.WithIdentityUser);

            user.IsBanned = true;

            user.IdentityUser.LockoutEnd = currentDateAndTime.AddYears(100);

            user.IdentityUser.LockoutEnabled = true;

            user.ModifiedOn = currentDateAndTime;

            await userManager
                .UpdateSecurityStampAsync(user.IdentityUser);

            await data.UpdateAsync(user);
        }

        /// <summary>
        /// Ubans the user by setting it's IsBanned property to false and marking his linked IdentityUser's LockoutEnabled property to "false"
        /// </summary>
        /// <param name="userId">BaseUser's Id</param>
        public async Task UnbanAsync(int userId)
        {
            var user = await data.GetByIdAsync(userId, UserQueryFilter.WithIdentityUser);

            user.IsBanned = false;

            user.IdentityUser.LockoutEnabled = false;

            user.ModifiedOn = DateTime.UtcNow;

            await data.UpdateAsync(user);
        }

        /// <summary>
        /// Checks if a user with the given Id exists in the database
        /// </summary>
        /// <param name="userId">User's Id</param>
        /// <returns>Bool - True if it exists and False if otherwise</returns>
        public async Task<bool> UserExistsAsync(int userId)
        {
            return await data.UserExistsAsync(userId);
        }

        /// <summary>
        /// Checks whether the user with the given Id is banned
        /// </summary>
        /// <param name="userId">User's Id</param>
        /// <returns>Bool - True if the user is banned, False if otherwise</returns>
        public async Task<bool> IsBannedAsync(int userId)
        {
            var user = await data.GetByIdAsync(userId, UserQueryFilter.AsNoTracking);
            return user.IsBanned;
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
