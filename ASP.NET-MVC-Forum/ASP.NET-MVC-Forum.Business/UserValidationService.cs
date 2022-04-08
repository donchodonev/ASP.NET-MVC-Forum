namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Business.Contracts.Contracts;
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Exceptions;
    using ASP.NET_MVC_Forum.Infrastructure.Extensions;

    using Microsoft.EntityFrameworkCore;

    using System.Security.Claims;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.ClientMessage.Error;
    using static ASP.NET_MVC_Forum.Domain.Constants.DataConstants;
    using static ASP.NET_MVC_Forum.Domain.Constants.RoleConstants;

    public class UserValidationService : IUserValidationService
    {
        private readonly ApplicationDbContext db;
        private readonly IUserRepository userRepo;

        public UserValidationService(ApplicationDbContext db,
            IUserRepository userRepo)
        {
            this.db = db;
            this.userRepo = userRepo;
        }

        public async Task ValidateUserIsPrivilegedAsync(int postId, ClaimsPrincipal user)
        {
            if (!await IsAuthor(user.Id(), postId) || !user.IsAdminOrModerator())
            {
                throw new InsufficientPrivilegeException(YOU_ARE_NOT_THE_AUTHER);
            }
        }

        public Task<bool> IsAuthor(string userId, int postId)
        {
            return db
                .Posts
                .AsNoTracking()
                .AnyAsync(x => x.Id == postId && x.UserId == userId);
        }

        public async Task<bool> IsUserPrivileged(int postId, ClaimsPrincipal currentPrincipal)
        {
            return await IsAuthor(currentPrincipal.Id(), postId) || currentPrincipal.IsAdminOrModerator();
        }

        public void ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new InvalidUsernameException(USERNAME_TOO_SHORT);
            }
            else if (username.Length < UserConstants.UsernameMinLength)
            {
                throw new InvalidUsernameException(USERNAME_TOO_SHORT);
            }
        }

        public void ValidateUserNotNull(ExtendedIdentityUser user)
        {
            throw new NullUserException(USER_DOES_NOT_EXIST);
        }

        public async Task ValidateUserExistsByUsernameAsync(string username)
        {
            if (!await userRepo.ExistsByUsernameAsync(username))
            {
                throw new NullUserException(USER_DOES_NOT_EXIST);
            }
        }

        public async Task ValidateUserExistsByIdAsync(string userId)
        {
            if (!await userRepo.ExistsByIdAsync(userId))
            {
                throw new NullUserException(USER_DOES_NOT_EXIST);
            }
        }

        public async Task ValidateUserIsNotBannedAsync(string userId)
        {
            if (await userRepo.IsBannedAsync(userId))
            {
                throw new UserIsBannedException(USER_ALREADY_BANNED);
            }
        }

        public async Task ValidateUserIsBannedAsync(string userId)
        {
            if (!await userRepo.IsBannedAsync(userId))
            {
                throw new UserIsBannedException(USER_NOT_YET_BANNED);
            }
        }

        public async Task ValidateUserIsNotModerator(ExtendedIdentityUser user)
        {
            if (await userRepo.IsInRoleAsync(user, MODERATOR_ROLE))
            {
                throw new InvalidRoleException(USER_IS_MODERATOR_ALREADY);
            }
        }

        public async Task ValidateUserIsModerator(string userId)
        {
            var user = await userRepo.GetByIdAsync(userId);

            if (!await userRepo.IsInRoleAsync(user, MODERATOR_ROLE))
            {
                throw new InvalidRoleException(CANNOT_FURTHER_DEMOTE);
            }
        }
    }
}
