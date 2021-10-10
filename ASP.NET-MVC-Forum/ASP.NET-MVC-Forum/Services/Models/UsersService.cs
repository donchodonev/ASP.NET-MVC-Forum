namespace ASP.NET_MVC_Forum.Services.Models
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Services.Contracts;
    using Microsoft.AspNetCore.Identity;
    using System.Threading.Tasks;

    public class UsersService : IUsersService
    {
        private readonly ApplicationDbContext db;

        public UsersService(ApplicationDbContext db)
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
    }
}
