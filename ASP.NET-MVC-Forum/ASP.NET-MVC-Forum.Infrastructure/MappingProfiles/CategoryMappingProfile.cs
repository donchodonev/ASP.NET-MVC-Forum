namespace ASP.NET_MVC_Forum.Infrastructure.MappingProfiles
{
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Post;
    using ASP.NET_MVC_Forum.Domain.Models.Stats;

    using AutoMapper;

    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            CreateMap<Category, CategoryIdAndNameViewModel>();

            CreateMap<Category, MostPostsPerCategoryResponseModel>()
                .ForMember(x => x.Count, y => y.MapFrom(y => y.Posts.Count))
                .ForMember(x => x.Title, y => y.MapFrom(y => y.Name));
        }
    }
}
