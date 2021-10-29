namespace ASP.NET_MVC_Forum.Services.User
{
    using Microsoft.AspNetCore.Identity;
    using System.Linq;
    using System.Threading.Tasks;
    using ASP.NET_MVC_Forum.Data.Models;
    public interface IUserService
    {
        /// <summary>
        /// Adds a new BaseType user to the database
        /// </summary>
        /// <param name="identityUser">Default IdentityUser the base used would link to</param>
        /// <param name="firstName">BaseUser FirstName property</param>
        /// <param name="lastName">BaseUser LastName property</param>
        /// <returns></returns>
        public Task<int> AddАsync(IdentityUser identityUser, string firstName, string lastName, int? age = null);

        public Task<int> GetBaseUserIdAsync(string identityUserId);

        public  Task<int> UserPostsCount(int userId);

        public IQueryable<User> GetAll(bool withIdentityIncluded = false);

        public bool UserExists(int userId);

        public bool IsBanned(int userId);

        public User GetUser(int userId);

        public void Ban(int userId);

        public void Unban(int userId);

        public User GetUser(string identityUserId);
    }
}
