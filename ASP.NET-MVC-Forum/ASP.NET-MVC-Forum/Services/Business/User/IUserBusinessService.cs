using ASP.NET_MVC_Forum.Areas.Admin.Models.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASP.NET_MVC_Forum.Services.Business.User
{
    public interface IUserBusinessService
    {
        public Task<List<UserViewModel>> GenerateUserViewModelAsync();

        public Task BanAsync(int userId);

        public Task UnbanAsync(int userId);
    }
}
