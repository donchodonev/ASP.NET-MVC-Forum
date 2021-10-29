namespace ASP.NET_MVC_Forum.Services.User
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Data.DataConstants.RoleConstants;

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<IdentityUser> userManager;

        public UserService(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        public async Task<int> AddАsync(IdentityUser identityUser, string firstName, string lastName, int? age = null)
        {
            var user = new User
            {
                IdentityUserId = identityUser.Id,
                IdentityUser = identityUser,
                FirstName = firstName,
                LastName = lastName,
                Age = age
            };
            await db.BaseUsers.AddAsync(user);

            return await db.SaveChangesAsync();
        }

        public async Task<int> GetBaseUserIdAsync(string identityUserId)
        {
            return await Task
               .Run(() => db
               .BaseUsers
               .FirstOrDefault(x => x.IdentityUserId == identityUserId).Id
               );
        }

        public async Task<int> UserPostsCount(int userId)
        {
            return await Task.Run(() =>
            {
                return db
                .BaseUsers
                .FirstOrDefault(x => x.Id == userId)
                .Posts
                .Count;
            });
        }

        public IQueryable<User> GetAll(bool withIdentityIncluded = false)
        {
            var query = db
                .BaseUsers
                .AsQueryable<User>();

            if (withIdentityIncluded)
            {
                query = query.Include(x => x.IdentityUser);
            }

            return query;
        }

        public bool UserExists(int userId)
        {
            return db
                .BaseUsers
                .Any(x => x.Id == userId);
        }

        public bool IsBanned(int userId)
        {
            return db
                .BaseUsers
                .First(x => x.Id == userId)
                .IsBanned;
        }

        public User GetUser(int userId, bool withIdentityUser = false)
        {
            var query = db
                .BaseUsers
                .Where(x => x.Id == userId);

            if (withIdentityUser)
            {
                query = query.Include(x => x.IdentityUser);
            }

            return query.SingleOrDefault();
        }

        public User GetUser(string identityUserId, bool withIdentityUser = false)
        {
            var query = db
                .BaseUsers
                .Where(x => x.IdentityUserId == identityUserId);

            if (withIdentityUser)
            {
                query = query.Include(x => x.IdentityUser);
            }

            return query.SingleOrDefault();
        }

        public void Ban(int userId)
        {
            var user = GetUser(userId, true);

            user.IsBanned = true;
            user.IdentityUser.LockoutEnd = DateTime.UtcNow.AddYears(100);
            user.IdentityUser.LockoutEnabled = true;

            userManager
                .UpdateSecurityStampAsync(user.IdentityUser)
                .GetAwaiter()
                .GetResult();

            db.Update<User>(user);
            db.Update<IdentityUser>(user.IdentityUser);
            db.SaveChanges();
        }

        public void Unban(int userId)
        {
            var user = GetUser(userId, true);
            user.IsBanned = false;
            user.IdentityUser.LockoutEnabled = false;
            db.Update<User>(user);
            db.Update<IdentityUser>(user.IdentityUser);
            db.SaveChanges();
        }

        public void Promote(IdentityUser user)
        {
            userManager
                .AddToRoleAsync(user, ModeratorRoleName)
                .GetAwaiter()
                .GetResult();
        }

        public void Demote(IdentityUser user)
        {
            userManager
                .RemoveFromRoleAsync(user,ModeratorRoleName)
                .GetAwaiter()
                .GetResult();
        }
    }
}
