namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Business.Contracts.Contracts;
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Domain.Exceptions;
    using ASP.NET_MVC_Forum.Infrastructure.Extensions;

    using Microsoft.EntityFrameworkCore;

    using System.Security.Claims;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.ClientMessage.Error;

    public class UserValidationService : IUserValidationService
    {
        private readonly ApplicationDbContext db;

        public UserValidationService(ApplicationDbContext db)
        {
            this.db = db;
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
    }
}
