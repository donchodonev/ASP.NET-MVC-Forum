namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Models.User;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;

    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUserBusinessService
    {
        public Task AvatarUpdateAsync(string identityUserId, IFormFile image);

        public Task<List<UserViewModel>> GenerateUserViewModelAsync();

        public Task BanAsync(int userId);

        public Task UnbanAsync(int userId);

        public Task<bool> IsBannedAsync(int userId);

        public Task<bool> UserExistsAsync(int userId);

        public Task<IList<string>> GetUserRolesAsync(int userId);

        public Task<IdentityUser> GetIdentityUser(int userId);

        public Task DemoteAsync(int userId);

        public Task PromoteAsync(int userId);

        public Task<int> UserPostsCountAsync(int userId);
    }
}
