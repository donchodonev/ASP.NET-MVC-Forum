namespace ASP.NET_MVC_Forum.Infrastructure.MappingProfiles
{
    using ASP.NET_MVC_Forum.Domain.Models.Chat;

    using AutoMapper;

    using static ASP.NET_MVC_Forum.Domain.Constants.DateTimeConstants;

    public class MessageMappingProfile : Profile
    {
        public MessageMappingProfile()
        {
            CreateMap<Domain.Entities.Message, ChatMessageResponseData>()
            .ForMember(x => x.SenderUsername, y => y.MapFrom(y => y.SenderUsername))
            .ForMember(x => x.Time, y => y.MapFrom(y => y.CreatedOn.AddHours(2) // FOR GMT+2
            .ToString(DateAndTimeFormat)));
        }
    }
}
