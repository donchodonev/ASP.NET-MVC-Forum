namespace ASP.NET_MVC_Forum.Services.User
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext db;

        public UserService(ApplicationDbContext db)
        {
            this.db = db;
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

        public User GetUser(int userId)
        {
            return db
                .BaseUsers
                .First(x => x.Id == userId);
        }

        public User GetUser(string identityUserId)
        {
            return db
                .BaseUsers
                .First(x => x.IdentityUserId == identityUserId);
        }

        public void Ban(int userId)
        {
            var user = GetUser(userId);
            user.IsBanned = true;
            db.Update<User>(user);
            db.SaveChanges();
        }

        public void Unban(int userId)
        {
            var user = GetUser(userId);
            user.IsBanned = false;
            db.Update<User>(user);
            db.SaveChanges();
        }
    }
}
