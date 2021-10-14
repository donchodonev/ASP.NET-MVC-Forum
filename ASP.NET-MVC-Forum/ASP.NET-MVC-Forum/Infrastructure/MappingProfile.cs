using ASP.NET_MVC_Forum.Data.Models;
using ASP.NET_MVC_Forum.Models.Category;
using ASP.NET_MVC_Forum.Models.Post;
using AutoMapper;

using static ASP.NET_MVC_Forum.Data.DataConstants.DateTimeFormat;

namespace ASP.NET_MVC_Forum.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Category, AllCategoryViewModel>()
                .ReverseMap();

            this.CreateMap<Category, CategoryIdAndName>();

            this.CreateMap<AddPostFormModel, Post>().ReverseMap();

            this.CreateMap<Post, EditPostFormModel>()
                .ForMember(x => x.PostId, y => y.MapFrom(z => z.Id))
                .ReverseMap();

            this.CreateMap<Post, PostPreviewViewModel>()
                .ForMember(x => x.UserPostsCount, y => y.MapFrom(z => z.User.Posts.Count))
                .ForMember(x => x.UserIdentityUserUsername, y => y.MapFrom(z => z.User.IdentityUser.UserName))
                .ForMember(x => x.UserImageUrl, y => y.MapFrom(z => z.User.ImageUrl))
                .ForMember(x => x.UserMemberSince, y => y.MapFrom(z => z.User.CreatedOn.ToString(DateFormat)))
                .ForMember(x => x.UserUsername, y => y.MapFrom(z => z.User.IdentityUser.UserName))
                .ForMember(x => x.PostCreationDate, y => y.MapFrom(z => z.CreatedOn.ToString(DateAndTimeFormat) + " UTC"))
                .ForMember(x => x.PostId, y => y.MapFrom(z => z.Id));

            this.CreateMap<Post, ViewPostViewModel>()
                .ForMember(x => x.UserPostsCount, y => y.MapFrom(z => z.User.Posts.Count))
                .ForMember(x => x.UserIdentityUserUsername, y => y.MapFrom(z => z.User.IdentityUser.UserName))
                .ForMember(x => x.UserImageUrl, y => y.MapFrom(z => z.User.ImageUrl))
                .ForMember(x => x.UserMemberSince, y => y.MapFrom(z => z.User.CreatedOn.ToString(DateFormat)))
                .ForMember(x => x.UserUsername, y => y.MapFrom(z => z.User.IdentityUser.UserName))
                .ForMember(x => x.PostCreationDate, y => y.MapFrom(z => z.CreatedOn.ToString(DateAndTimeFormat) + " UTC"))
                .ForMember(x => x.PostId, y => y.MapFrom(z => z.Id));
        }
    }
}
