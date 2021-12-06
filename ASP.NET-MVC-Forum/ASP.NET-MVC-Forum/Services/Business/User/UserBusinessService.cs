namespace ASP.NET_MVC_Forum.Services.Business.User
{
    using ASP.NET_MVC_Forum.Areas.Admin.Models.User;
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Services.User;
    using AutoMapper;
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class UserBusinessService : IUserBusinessService
    {
        private readonly IUserDataService userDataService;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IMapper mapper;

        public UserBusinessService(IUserDataService userService, UserManager<IdentityUser> userManager, IMapper mapper)
        {
            this.userDataService = userService;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<List<UserViewModel>> GenerateUserViewModelAsync()
        {
            var allUsers = userDataService.GetAll(UserQueryFilter.WithIdentityUser, UserQueryFilter.AsNoTracking);

            var vm = mapper
                .Map<List<UserViewModel>>(allUsers)
                .ToList();

            return await ReturnUsersWithRoles(vm);
        }

        private async Task<List<UserViewModel>> ReturnUsersWithRoles(List<UserViewModel> users)
        {
            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user.IdentityUser);

                user.Roles = roles.ToList();
            }

            return users;
        }
    }
}
