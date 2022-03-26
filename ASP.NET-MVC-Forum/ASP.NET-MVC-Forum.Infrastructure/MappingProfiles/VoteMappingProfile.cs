namespace ASP.NET_MVC_Forum.Infrastructure.MappingProfiles
{
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Votes;

    using AutoMapper;

    public class VoteMappingProfile : Profile
    {
        public VoteMappingProfile()
        {
            CreateMap<VoteRequestModel, Vote>()
            .ForMember(x => x.VoteType, y => y.MapFrom(z => z.IsPositiveVote ? 1 : -1));
        }
    }
}
