﻿namespace ASP.NET_MVC_Forum.Business
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.User;
    using ASP.NET_MVC_Forum.Validation.Contracts;

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
        private readonly IUserValidationService userValidationService;
        private readonly IMapper mapper;
        private readonly UserManager<ExtendedIdentityUser> userManager;

        public UserService(
            IUserRepository userRepo,
            IUserValidationService userValidationService,
            IMapper mapper,
            UserManager<ExtendedIdentityUser> userManager)
        {
            this.userRepo = userRepo;
            this.userValidationService = userValidationService;
            this.mapper = mapper;
            this.userManager = userManager;
        }
        public async Task AvatarUpdateAsync(string identityUserId, IFormFile image)
        {
            await userRepo.AvatarUpdateAsync(identityUserId, image);
        }

        public async Task<ExtendedIdentityUser> GetByUsernameAsync(string username)
        {
            userValidationService.ValidateUsername(username);

            var user = await userRepo.GetByUsernameAsync(username);

            userValidationService.ValidateUserNotNull(user);

            return user;
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
            await userValidationService.ValidateUserExistsByIdAsync(userId);

            await userValidationService.ValidateUserIsNotBannedAsync(userId);

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
            await userValidationService.ValidateUserExistsByIdAsync(userId);

            await userValidationService.ValidateUserIsBannedAsync(userId);

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
            return userRepo.ExistsByIdAsync(userId);
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

        public async Task DemoteAsync(string userId)
        {
            await userValidationService.ValidateUserExistsByIdAsync(userId);

            await userValidationService.ValidateUserIsModerator(userId);

            await userRepo.RemoveRoleAsync(userId, MODERATOR_ROLE);
        }

        public async Task PromoteAsync(string userId)
        {
            var user = await userRepo.GetByIdAsync(userId);

            userValidationService.ValidateUserNotNull(user);

            await userValidationService.ValidateUserIsNotModerator(user);

            await userRepo.AddRoleAsync(userId, MODERATOR_ROLE);
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
            return userRepo.IsInRoleAsync(user, role);
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
