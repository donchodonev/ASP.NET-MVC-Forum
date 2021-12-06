namespace ASP.NET_MVC_Forum.Services.User
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Services.Business.UserAvatar;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;
    using static ASP.NET_MVC_Forum.Data.Constants.WebConstants;

    public class UserDataService : IUserDataService
    {
        private readonly ApplicationDbContext db;
        private readonly IUserAvatarService avatarService;

        public UserDataService(ApplicationDbContext db, IUserAvatarService avatarService)
        {
            this.db = db;
            this.avatarService = avatarService;
        }

        /// <summary>
        /// Adds a new BaseUser to the database asynchronously and links it to it's unique ASP.NET Core IdentityUser
        /// </summary>
        /// <param name="identityUser">ASP.NET Core IdentityUser</param>
        /// <param name="firstName">User's first name</param>
        /// <param name="lastName">User's last name</param>
        /// <param name="age">User's age (optional)</param>
        /// <returns>Task<int> - the number of state entries written to the database</returns>
        public async Task<int> AddАsync(IdentityUser identityUser, string firstName, string lastName, int? age = null)
        {
            var user = new User
            {
                IdentityUserId = identityUser.Id,
                IdentityUser = identityUser,
                FirstName = firstName,
                LastName = lastName,
                Age = age,
                ImageUrl = AvatarURL
            };

            await db.BaseUsers.AddAsync(user);

            return await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            db.Update(user);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Get's BaseUser's Id using IdentityUser's Id
        /// </summary>
        /// <param name="identityUserId"></param>
        /// <returns>Task<int></returns>
        public async Task<int> GetBaseUserIdAsync(string identityUserId)
        {
            var user = await db
               .BaseUsers
               .AsNoTracking()
               .FirstOrDefaultAsync(x => x.IdentityUserId == identityUserId);

            return user.Id;
        }

        /// <summary>
        /// Get's BaseUser's posts count
        /// </summary>
        /// <param name="userId">BaseUser's Id</param>
        /// <returns></returns>
        public async Task<int> UserPostsCount(int userId)
        {
            var user = await db
             .BaseUsers
             .AsNoTracking()
             .FirstOrDefaultAsync(x => x.Id == userId);

            return user.Posts.Count;
        }

        /// <summary>
        /// Gets all users filtered by selected filters (if any)
        /// </summary>
        /// <param name="filters"> Desired filters of type UserQueryFilter</param>
        /// <returns>IQueryable<User></returns>
        public IQueryable<User> GetAll(params UserQueryFilter[] filters)
        {
            var query = QueryBuilder(filters);

            return query;
        }

        /// <summary>
        /// Checks if a user with the given Id exists in the database
        /// </summary>
        /// <param name="userId">User's Id</param>
        /// <returns>Bool - True if it exists and False if otherwise</returns>
        public async Task<bool> UserExistsAsync(int userId)
        {
            return await db
                .BaseUsers
                .AsNoTracking()
                .AnyAsync(x => x.Id == userId);
        }

        /// <summary>
        /// Gets a user based on his Id and provided filters (if any)
        /// </summary>
        /// <param name="userId">User's Id</param>
        /// <param name="filters">Zero or more filters which include more user data</param>
        /// <returns>User</returns>
        public IQueryable<User> GetUser(int userId, params UserQueryFilter[] filters)
        {
            var query = db
                .BaseUsers
                .Where(x => x.Id == userId);

            return QueryBuilder(query, filters);
        }

        /// <summary>
        /// Gets a user based on his IdentityUser Id and provided filters (if any)
        /// </summary>
        /// <param name="userId">User's Id</param>
        /// <param name="filters">Zero or more filters which include more user data</param>
        /// <returns>User</returns>
        public IQueryable<User> GetUser(string identityUserId, params UserQueryFilter[] filters)
        {
            var query = db
                .BaseUsers
                .Where(x => x.IdentityUserId == identityUserId);

            return QueryBuilder(query, filters);
        }

        /// <summary>
        /// Sets the user's avatar to the default avatar image
        /// </summary>
        /// <param name="identityUserId"></param>
        public async Task AvatarDeleteAsync(string identityUserId)
        {
            var user = await GetByIdAsync(identityUserId);

            user.ImageUrl = AvatarURL;

            db.Update(user);

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Facade method - uploads image and links it to user, then sets that image as the user's new avatar and persists the change
        /// </summary>
        /// <param name="identityUserId">IdentityUser's Id</param>
        /// <param name="image">The image file</param>
        public async Task AvatarUpdateAsync(string identityUserId, IFormFile image)
        {
            string fileName = await avatarService
                .UploadAvatarAsync(image);

            var user = await GetByIdAsync(identityUserId);

            user.ImageUrl = $"{AvatarWebPath}{fileName}";

            db.Update(user);

            db.SaveChanges();
        }

        /// <summary>
        /// Checks whether the user as an avatar different from the default one
        /// </summary>
        /// <param name="userId">BaseUser's Id</param>
        /// <returns>True if the user has an avatar different from the default one, False if otherwise</returns>
        public bool UserHasAvatar(int userId)
        {
            return GetUser(userId, UserQueryFilter.AsNoTracking, UserQueryFilter.WithoutDeleted)
                .First(x => x.Id != userId).ImageUrl != AvatarURL;
        }

        /// <summary>
        /// Gets user's avatar URL
        /// </summary>
        /// <param name="identityUserId">IIdentityUser's Id</param>
        /// <returns>string - user's avatar URL</returns>
        public async Task<string> GetUserAvatarAsync(string identityUserId)
        {
            var user = await GetByIdAsync(identityUserId);

            return user.ImageUrl;
        }

        /// <summary>
        /// Get's image's extension
        /// </summary>
        /// <param name="image">Image file</param>
        /// <returns>Image's extension if valid, null if otherwise</returns>
        public string GetImageExtension(IFormFile image)
        {
            return avatarService.GetImageExtension(image);
        }

        public async Task<User> GetByIdAsync(int userId, params UserQueryFilter[] userQueryFilters)
        {
            return await GetUser(userId, userQueryFilters)
                .FirstOrDefaultAsync();
        }

        public async Task<User> GetByIdAsync(string identityUserId, params UserQueryFilter[] userQueryFilters)
        {
            return await GetUser(identityUserId,userQueryFilters).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Builds database query based on provided filters and pre-defined user db query to the User's table
        /// </summary>
        /// <param name="users">Pre-defined user db query to the User's table</param>
        /// <param name="filters">Desired filters of type UserQueryFilter</param></param>
        /// <returns>IQueryable<User></returns>
        private IQueryable<User> QueryBuilder(IQueryable<User> users, params UserQueryFilter[] filters)
        {
            if (users.Count() == 0)
            {
                return users;
            }

            foreach (var filter in filters)
            {
                switch (filter)
                {
                    case UserQueryFilter.WithoutDeleted:
                        users = users.Where(x => x.IsDeleted == false);
                        break;
                    case UserQueryFilter.AsNoTracking:
                        users = users.AsNoTracking();
                        break;
                    case UserQueryFilter.WithIdentityUser:
                        users = users.Include(x => x.IdentityUser);
                        break;
                }
            }

            return users;
        }
        /// <summary>
        /// Builds database query to the User's table based on the provided filters in the method constructor
        /// </summary>
        /// <param name="filters">Desired filters of type UserQueryFilter</param>
        /// <returns>IQueryable<User></returns>
        private IQueryable<User> QueryBuilder(params UserQueryFilter[] filters)
        {
            var query = db
            .BaseUsers
            .AsQueryable();

            foreach (var filter in filters)
            {
                switch (filter)
                {
                    case UserQueryFilter.WithoutDeleted:
                        query = query.Where(x => x.IsDeleted == false);
                        break;
                    case UserQueryFilter.AsNoTracking:
                        query = query.AsNoTracking();
                        break;
                    case UserQueryFilter.WithIdentityUser:
                        query = query.Include(x => x.IdentityUser);
                        break;
                }
            }

            return query;
        }
    }
}
