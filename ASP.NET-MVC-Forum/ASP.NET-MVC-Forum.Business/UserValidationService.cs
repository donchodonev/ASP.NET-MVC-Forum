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

        public void ValidateNotNull(ExtendedIdentityUser user)
        {
            throw new NullUserException(USER_DOES_NOT_EXIST);
        }

        public async Task ValidateUserExistsAsync(string username)
        {
            if (!await userRepo.ExistsAsync(username))
            {
                throw new NullUserException(USER_DOES_NOT_EXIST);
            }
        }
    }
}
