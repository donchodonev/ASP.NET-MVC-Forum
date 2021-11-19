namespace ASP.NET_MVC_Forum.Services.User
{
    using Microsoft.AspNetCore.Identity;
    using System.Linq;
    using System.Threading.Tasks;
    using ASP.NET_MVC_Forum.Data.Models;
    using Microsoft.AspNetCore.Http;
    using ASP.NET_MVC_Forum.Data.Enums;

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

        public Task<int> UserPostsCount(int userId);

        public IQueryable<User> GetAll(params UserQueryFilter[] filters);

        public bool UserExists(int userId);

        public bool IsBanned(int userId);

        public IQueryable<User> GetUser(int userId, params UserQueryFilter[] filters);

        public IQueryable<User> GetUser(string identityUserId, params UserQueryFilter[] filters);

        public void Ban(int userId);

        public void Unban(int userId);

        public void Promote(IdentityUser user);

        public void Demote(IdentityUser user);

        public void AvatarDelete(string identityUserId);

        public bool UserHasAvatar(int userId);

        public string GetUserAvatar(string identityUserId);

        /// <summary>
        /// Gets said image's extension if extension is amongst the allowed image extensions defined at ASP.NET_MVC_Forum.Data.DataConstants.AllowedImageExtensions that are used in the implementation of the GetImageExtension() method in UserAvatarService
        /// 
        public string GetImageExtension(IFormFile image);

        public void AvatarUpdate(string identityUserId, IFormFile image);
    }
}
