namespace ASP.NET_MVC_Forum.Services.Business.User
{
    using ASP.NET_MVC_Forum.Services.User;
    public class UserBusinessService : IUserBusinessService
    {
        private readonly IUserService userService;

        public UserBusinessService(IUserService userService)
        {
            this.userService = userService;
        }
    }
}
