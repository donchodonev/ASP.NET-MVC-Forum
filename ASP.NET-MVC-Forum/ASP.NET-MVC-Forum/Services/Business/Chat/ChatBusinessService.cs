using ASP.NET_MVC_Forum.Data.Enums;
using ASP.NET_MVC_Forum.Models.Chat;
using ASP.NET_MVC_Forum.Services.Data.User;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NET_MVC_Forum.Services.Business.Chat
{
    public class ChatBusinessService : IChatBusinessService
    {
        private readonly IMapper mapper;
        private readonly IUserDataService data;
        private readonly UserManager<IdentityUser> userManager;

        public ChatBusinessService(IMapper mapper, IUserDataService data, UserManager<IdentityUser> userManager)
        {
            this.mapper = mapper;
            this.data = data;
            this.userManager = userManager;
        }

        public async Task<ChatSelectUserViewModel> GenerateChatSelectUserViewModel(string recipientUsername, string currentIdentityUserId, string currentIdentityUserUsername)
        {
            var identityUser = await userManager.FindByNameAsync(recipientUsername);

            var vm = await mapper
                .ProjectTo<ChatSelectUserViewModel>(data.GetUser(identityUser.Id, UserQueryFilter.AsNoTracking, UserQueryFilter.WithIdentityUser))
                .FirstAsync();

            vm.SenderUsername = currentIdentityUserUsername;
            vm.SenderIdentityUserId = currentIdentityUserId;

            return vm;
        }
    }
}
