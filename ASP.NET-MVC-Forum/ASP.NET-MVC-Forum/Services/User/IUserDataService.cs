namespace ASP.NET_MVC_Forum.Services.User
{
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Data.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IUserDataService
    {
        public Task UpdateAsync(User user);

        public Task<User> GetByIdAsync(string identityUserId, params UserQueryFilter[] userQueryFilters);

        public Task<User> GetByIdAsync(int userId, params UserQueryFilter[] userQueryFilters);

        /// <summary>
        /// Adds a new BaseType user to the database
        /// </summary>
        /// <param name="identityUser">Default IdentityUser the base used would link to</param>
        /// <param name="firstName">BaseUser FirstName property</param>
        /// <param name="lastName">BaseUser LastName property</param>
        /// <returns></returns>
        public Task<int> AddАsync(IdentityUser identityUser, string firstName, string lastName, int? age = null);

        public Task<int> GetBaseUserIdAsync(string identityUserId);

        public IQueryable<User> GetAll(params UserQueryFilter[] filters);

        public Task<bool> UserExistsAsync(int userId);

        public IQueryable<User> GetUser(int userId, params UserQueryFilter[] filters);

        public IQueryable<User> GetUser(string identityUserId, params UserQueryFilter[] filters);

        public Task AvatarDeleteAsync(string identityUserId);

        public bool UserHasAvatar(int userId);

        public Task<string> GetUserAvatarAsync(string identityUserId);

        /// <summary>
        /// Gets said image's extension if extension is amongst the allowed image extensions defined at ASP.NET_MVC_Forum.Data.DataConstants.AllowedImageExtensions that are used in the implementation of the GetImageExtension() method in UserAvatarService
        /// 
        public string GetImageExtension(IFormFile image);

        public Task AvatarUpdateAsync(string identityUserId, IFormFile image);
    }
}
