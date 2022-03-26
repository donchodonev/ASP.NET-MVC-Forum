namespace ASP.NET_MVC_Forum.Infrastructure.MappingProfiles
{
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.CommentReport;

    using AutoMapper;

    public class CommentReportMappingProfile : Profile
    {
        public CommentReportMappingProfile()
        {
            CreateMap<CommentReport, CommentReportViewModel>()
.               ForMember(x => x.CommentContent, y => y.MapFrom(z => z.Comment.Content));
        }
    }
}
