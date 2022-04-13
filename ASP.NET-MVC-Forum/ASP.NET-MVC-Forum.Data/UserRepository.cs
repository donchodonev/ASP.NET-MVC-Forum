namespace ASP.NET_MVC_Forum.Data
{
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.WebConstants;

    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ExtendedIdentityUser> userManager;
        private readonly IAvatarRepository avatarService;

        public UserRepository(
            ApplicationDbContext db,
            UserManager<ExtendedIdentityUser> userManager,
            IAvatarRepository avatarService)
        {
            this.db = db;
            this.userManager = userManager;
            this.avatarService = avatarService;
        }

        public Task AddАsync(
            string firstName,
            string lastName,
            string password,
            string email,
            string userName,
            int? age = null)
        {
            var user = new ExtendedIdentityUser
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Age = age,
                Email = email,
                ImageUrl = AVATAR_URL
            };

            return userManager.CreateAsync(user, password);
        }

        public Task UpdateAsync(ExtendedIdentityUser user)
        {
            return userManager.UpdateAsync(user);
        }

        public async Task<bool> ExistsByIdAsync(string userId)
        {
            return (await userManager.FindByIdAsync(userId)) != null;
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return (await userManager.FindByNameAsync(username)) != null;
        }

        public IQueryable<ExtendedIdentityUser> GetAll()
        {
            return db.Users;
        }

        public Task<bool> IsAuthor(string userId, int postId)
        {
            return db
                .Posts
                .AsNoTracking()
                .AnyAsync(x => x.Id == postId && x.UserId == userId);
        }

        public IQueryable<ExtendedIdentityUser> GetAllAsNoTracking()
        {
            return GetAll().AsNoTracking();
        }

        public IQueryable<ExtendedIdentityUser> GetById(string userId)
        {
            return db.Users.Where(x => x.Id == userId);
        }

        public Task<bool> IsBannedAsync(string userId)
        {
            return
                GetById(userId)
                .Select(x => x.IsBanned)
                .FirstOrDefaultAsync();
        }

        public Task<ExtendedIdentityUser> GetByIdAsync(string userId)
        {
            return userManager.FindByIdAsync(userId);
        }

        public Task<ExtendedIdentityUser> GetByEmailAsync(string email)
        {
            return userManager.FindByEmailAsync(email);
        }

        public IQueryable<ExtendedIdentityUser> GetByUsername(string username)
        {
            return db
                .Users
                .Where(x => x.UserName == username);
        }

        public Task<ExtendedIdentityUser> GetByUsernameAsync(string username)
        {
            return userManager.FindByNameAsync(username);
        }

        public async Task ResetAvatarAsync(string identityUserId)
        {
            var user = await GetByIdAsync(identityUserId);

            user.ImageUrl = AVATAR_URL;

            db.Update(user);

            await db.SaveChangesAsync();
        }

        public async Task AvatarUpdateAsync(ExtendedIdentityUser user, IFormFile image)
        {
            string fileName = await avatarService.UploadAvatarAsync(image);

            user.ImageUrl = $"{AVATAR_WEB_PATH}{fileName}";

            db.Update(user);

            await db.SaveChangesAsync();
        }

        public bool IsAvatarDefault(string userId)
        {
            string userAvatarPath = db
                .Users
                .Where(x => x.Id == userId)
                .Select(x => x.ImageUrl)
                .First();

            return userAvatarPath == AVATAR_URL;
        }

        public Task<string> GetAvatarAsync(string identityUserId)
        {
            return db
                .Users
                .Where(x => x.Id == identityUserId)
                .Select(x => x.ImageUrl)
                .FirstOrDefaultAsync();
        }

        public Task<IList<string>> GetRolesAsync(ExtendedIdentityUser user)
        {
            return userManager.GetRolesAsync(user);
        }

        public async Task RemoveRoleAsync(string userId, string roleName)
        {
            var identityUser = await GetByIdAsync(userId);

            await userManager.RemoveFromRoleAsync(identityUser, roleName);
        }

        public async Task AddRoleAsync(string userId, string roleName)
        {
            var identityUser = await GetByIdAsync(userId);

            await userManager.AddToRoleAsync(identityUser, roleName);
        }

        public Task<bool> IsInRoleAsync(ExtendedIdentityUser user, string role)
        {
            return userManager.IsInRoleAsync(user, role);
        }

        public string GetImageExtension(IFormFile image)
        {
            return avatarService.GetImageExtension(image);
        }

        public ValueTask DisposeAsync()
        {
            return db.DisposeAsync();
        }
    }
}
