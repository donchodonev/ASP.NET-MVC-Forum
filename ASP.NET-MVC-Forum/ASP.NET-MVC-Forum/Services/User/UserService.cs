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
                return db.BaseUsers.FirstOrDefault(x => x.Id == userId).Posts.Count;
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
    }
}
