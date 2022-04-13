namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.User;

    using Microsoft.AspNetCore.Http;

    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUserService
    {
        public Task AvatarUpdateAsync(string identityUserId, IFormFile image);

        public Task<List<UserViewModel>> GenerateUserViewModelAsync();

        public Task BanAsync(string userId);

        public Task UnbanAsync(string userId);

        public Task DemoteAsync(string userId);

        public Task PromoteAsync(string userId);
    }
}
