namespace ASP.NET_MVC_Blog.Services.Models
{
    using ASP.NET_MVC_Blog.Data;
    using ASP.NET_MVC_Blog.Data.Models;
    using ASP.NET_MVC_Blog.Services.Contracts;
    using Microsoft.AspNetCore.Identity;
    using System.Threading.Tasks;

    public class UsersService : IUsersService
    {
        private readonly ApplicationDbContext db;

        public UsersService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<int> AddUserАsync(IdentityUser identityUser, string firstName, string lastName)
        {
            var user = new User
            {
                IdentityUserId = identityUser.Id,
                IdentityUser = identityUser,
                FirstName = firstName,
                LastName = lastName
            };
            await db.BaseUsers.AddAsync(user);

            return await db.SaveChangesAsync();
        }
    }
}
