namespace ASP.NET_MVC_Forum.Data.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;

    using Microsoft.AspNetCore.Http;

    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IUserRepository
    {
        public Task AddАsync(
            string firstName,
            string lastName,
            string password,
            string email,
            string userName,
            int? age = null);

        public Task UpdateAsync(ExtendedIdentityUser user);

        public Task<bool> ExistsByIdAsync(string userId);

        public Task<bool> IsAuthor(string userId, int postId);

        public Task<bool> ExistsByUsernameAsync(string username);

        public Task<bool> IsBannedAsync(string userId);

        public IQueryable<ExtendedIdentityUser> GetAll();

        public IQueryable<ExtendedIdentityUser> GetAllAsNoTracking();

        public IQueryable<ExtendedIdentityUser> GetById(string userId);

        public Task<ExtendedIdentityUser> GetByIdAsync(string userId);

        public Task<ExtendedIdentityUser> GetByEmailAsync(string email);

        public IQueryable<ExtendedIdentityUser> GetByUsername(string username);

        public Task<ExtendedIdentityUser> GetByUsernameAsync(string username);

        public Task ResetAvatarAsync(string identityUserId);

        public Task AvatarUpdateAsync(ExtendedIdentityUser user, IFormFile image);

        public bool IsAvatarDefault(string userId);

        public Task<string> GetAvatarAsync(string identityUserId);

        public Task RemoveRoleAsync(string userId, string roleName);

        public Task AddRoleAsync(string userId, string roleName);

        public Task<bool> IsInRoleAsync(ExtendedIdentityUser user, string role);

        public Task<IList<string>> GetRolesAsync(ExtendedIdentityUser user);

        public string GetImageExtension(IFormFile image);

        public ValueTask DisposeAsync();
    }
}
