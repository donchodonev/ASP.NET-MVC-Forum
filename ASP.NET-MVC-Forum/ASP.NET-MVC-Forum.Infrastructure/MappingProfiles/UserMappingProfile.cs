namespace ASP.NET_MVC_Forum.Infrastructure.MappingProfiles
{
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Chat;
    using ASP.NET_MVC_Forum.Domain.Models.User;

    using AutoMapper;

    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserViewModel>();

            CreateMap<User, ChatSelectUserViewModel>()
            .ForMember(x => x.RecipientUsername, cfg => cfg.MapFrom(y => y.IdentityUser.UserName))
            .ForMember(x => x.RecipientIdentityUserId, cfg => cfg.MapFrom(y => y.IdentityUser.Id));
        }
    }
}
