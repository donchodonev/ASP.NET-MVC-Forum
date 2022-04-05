namespace ASP.NET_MVC_Forum.Business
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.User;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.RoleConstants;

    public class UserService : IUserService
    {
        private readonly IUserRepository userRepo;
        private readonly UserManager<ExtendedIdentityUser> userManager;
        private readonly IMapper mapper;

        public async Task AvatarUpdateAsync(string identityUserId, IFormFile image)
        {
            await userRepo.AvatarUpdateAsync(identityUserId, image);
        }

        public UserService(
            IUserRepository userRepo,
            UserManager<ExtendedIdentityUser> userManager, 
            IMapper mapper)
        {
            this.userRepo = userRepo;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<List<UserViewModel>> GenerateUserViewModelAsync()
        {
            var vm = await userRepo
                    .GetAll()
                    .ProjectTo<UserViewModel>(mapper.ConfigurationProvider)
                    .ToListAsync();

            return await ReturnUsersWithRolesAsync(vm);
        }

        public async Task BanAsync(string userId)
        {
            var currentDateAndTime = DateTime.UtcNow;

            var user = await userRepo.GetByIdAsync(userId);

            user.IsBanned = true;

            user.LockoutEnd = currentDateAndTime.AddYears(100);

            user.LockoutEnabled = true;

            user.ModifiedOn = currentDateAndTime;

            await userManager
                .UpdateSecurityStampAsync(user);

            await userRepo.UpdateAsync(user);
        }

        /// <summary>
        /// Ubans the user by setting it's IsBanned property to false and marking his linked IdentityUser's LockoutEnabled property to "false"
        /// </summary>
        /// <param name="userId">BaseUser's Id</param>
        public async Task UnbanAsync(string userId)
        {
            var user = await userRepo.GetByIdAsync(userId);

            user.IsBanned = false;

            user.LockoutEnabled = false;

            user.ModifiedOn = DateTime.UtcNow;

            await userRepo.UpdateAsync(user);
        }

        /// <summary>
        /// Checks if a user with the given Id exists in the database
        /// </summary>
        /// <param name="userId">User's Id</param>
        /// <returns>Bool - True if it exists and False if otherwise</returns>
        public Task<bool> ExistsAsync(string userId)
        {
            return userRepo.ExistsAsync(userId);
        }

        /// <summary>
        /// Checks whether the user with the given Id is banned
        /// </summary>
        /// <param name="userId">User's Id</param>
        /// <returns>Bool - True if the user is banned, False if otherwise</returns>
        public Task<bool> IsBannedAsync(string userId)
        {
            return userRepo
                .GetById(userId)
                .Select(x => x.IsBanned)
                .FirstOrDefaultAsync();
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await userRepo.GetByIdAsync(userId);

            return await userRepo.GetRolesAsync(user);
        }

        public Task<IList<string>> GetUserRolesAsync(ExtendedIdentityUser user)
        {
            return userRepo.GetRolesAsync(user);
        }

        public Task DemoteAsync(string userId)
        {
            return userRepo.RemoveRoleAsync(userId, ModeratorRoleName);
        }

        public Task PromoteAsync(string userId)
        {
            return userRepo.AddRoleAsync(userId, ModeratorRoleName);
        }

        public Task<int> UserPostsCountAsync(string userId)
        {
            return userRepo
                .GetById(userId)
                .Where(x => !x.IsDeleted)
                .Include(x => x.Posts)
                .Select(x => x.Posts.Count)
                .FirstAsync();
        }

        public Task<bool> IsUserInRoleAsync(ExtendedIdentityUser user, string role)
        {
            return userManager.IsInRoleAsync(user, role);
        }

        private async Task<List<UserViewModel>> ReturnUsersWithRolesAsync(List<UserViewModel> users)
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
