using ASP.NET_MVC_Forum.Data.Models;
using ASP.NET_MVC_Forum.Models.Category;
using AutoMapper;

namespace ASP.NET_MVC_Forum.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Category,AllCategoryViewModel>()
                .ReverseMap();
        }
    }
}
