namespace ASP.NET_MVC_Forum.Infrastructure.MappingProfiles
{
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.PostReport;

    using AutoMapper;

    public class PostReportMappingProfile : Profile
    {
        public PostReportMappingProfile()
        {
           CreateMap<PostReport, PostReportViewModel>();
        }
    }
}
