using ASP.NET_MVC_Forum.Data.Models;
using ASP.NET_MVC_Forum.Models.Category;
using ASP.NET_MVC_Forum.Models.Post;
using AutoMapper;
using System.Web;
using Ganss.XSS;
using System.Text.RegularExpressions;



namespace ASP.NET_MVC_Forum.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Category,AllCategoryViewModel>()
                .ReverseMap();

            this.CreateMap<Category,CategoryIdAndName>();

            this.CreateMap<AddPostFormModel, Post>();

            this.CreateMap<Post,PostPreviewViewModel>()
                .ForMember(x => x.PostsCount, y=> y.MapFrom(z => z.User.Posts.Count))
                .ForMember(x => x.UserIdentityUserUsername, y => y.MapFrom(z => z.User.IdentityUser.UserName))
                .ForMember(x => x.UserImageUrl, y => y.MapFrom(z => z.User.ImageUrl))
                .ForMember(x => x.ShortDescription, y=> y.MapFrom(z => HttpUtility.HtmlDecode(z.HtmlContent)));
        }
    }
}
