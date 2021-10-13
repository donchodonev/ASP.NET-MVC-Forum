﻿namespace ASP.NET_MVC_Forum.Services.User
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Models;
    using Microsoft.AspNetCore.Identity;
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
    }
}