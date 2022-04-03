namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.User;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;

    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUserBusinessService
    {
        public Task AvatarUpdateAsync(string identityUserId, IFormFile image);

        public Task<List<UserViewModel>> GenerateUserViewModelAsync();

        public Task BanAsync(string userId);

        public Task UnbanAsync(string userId);

        public Task<bool> IsBannedAsync(string userId);

        public Task<IList<string>> GetUserRolesAsync(string userId);

        public Task DemoteAsync(string userId);

        public Task PromoteAsync(string userId);

        public Task<int> UserPostsCountAsync(string userId);

        public Task<bool> ExistsAsync(string userId);

        public Task<bool> IsUserInRoleAsync(ExtendedIdentityUser user, string role);
    }
}
