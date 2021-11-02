using ASP.NET_MVC_Forum.Areas.Admin.Models.Report;
using ASP.NET_MVC_Forum.Areas.Admin.Models.User;
using ASP.NET_MVC_Forum.Areas.API.Models;
using ASP.NET_MVC_Forum.Data.Models;
using ASP.NET_MVC_Forum.Models.Category;
using ASP.NET_MVC_Forum.Models.Post;
using ASP.NET_MVC_Forum.Services.Comment.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using static ASP.NET_MVC_Forum.Data.DataConstants.DateTimeFormat;

namespace ASP.NET_MVC_Forum.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<User,UserViewModel>();

            this.CreateMap<PostReport,ReportViewModel>();

            this.CreateMap<CommentPostRequestModel,RawCommentServiceModel>();

            this.CreateMap<Category, AllCategoryViewModel>()
                .ReverseMap();

            this.CreateMap<Category, CategoryIdAndName>();

            this.CreateMap<Comment, CommentGetRequestResponseModel>()
                .ForMember(x => x.CommentAuthor, y => y.MapFrom(z => z.User.IdentityUser.UserName));

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

                
            this.CreateMap<RawCommentServiceModel, Comment>()
                .ForMember(x => x.UserId, y => y.MapFrom(z => z.UserId))
                .ForMember(x => x.PostId, y => y.MapFrom(z => z.PostId))
                .ForMember(x => x.Content, y => y.MapFrom(z => z.CommentText));
        }
    }
}
