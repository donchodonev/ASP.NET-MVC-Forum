using ASP.NET_MVC_Forum.Areas.Admin.Models.CommentReport;
using ASP.NET_MVC_Forum.Areas.Admin.Models.PostReport;
using ASP.NET_MVC_Forum.Areas.Admin.Models.User;
using ASP.NET_MVC_Forum.Areas.API.Models.Comments;
using ASP.NET_MVC_Forum.Areas.API.Models.Stats;
using ASP.NET_MVC_Forum.Areas.API.Models.Votes;
using ASP.NET_MVC_Forum.Data.Models;
using ASP.NET_MVC_Forum.Models.Category;
using ASP.NET_MVC_Forum.Models.Post;
using ASP.NET_MVC_Forum.Services.Comment.Models;
using AutoMapper;
using System.Linq;
using static ASP.NET_MVC_Forum.Data.DataConstants.DateTimeFormat;

namespace ASP.NET_MVC_Forum.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Post, MostReportedPostsResponeModel>()
                .ForMember(x => x.Count, y => y.MapFrom(y => y.Reports.Count));

            this.CreateMap<Post, MostLikedPostsResponeModel>()
                .ForMember(x => x.Count, y => y.MapFrom(y => y.Votes.Sum(x => (int)x.VoteType)));

            this.CreateMap<Post, MostCommentedPostsResponeModel>()
                .ForMember(x => x.Count, y => y.MapFrom(y => y.Comments.Count));

            this.CreateMap<VoteRequestModel, Vote>()
                .ForMember(x => x.VoteType, y => y.MapFrom(z => z.IsPositiveVote ? 1 : -1));

            this.CreateMap<CommentReport, CommentReportViewModel>()
                .ForMember(x => x.CommentContent, y => y.MapFrom(z => z.Comment.Content));

            this.CreateMap<User,UserViewModel>();

            this.CreateMap<PostReport,PostReportViewModel>();

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
                .ForMember(x => x.CommentsCount, y => y.MapFrom(z => z.Comments.Where(x => x.IsDeleted == false).Count()))
                .ForMember(x => x.PostId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.VoteSum, y => y.MapFrom(z => z.Votes.Sum(x => (int)x.VoteType)));


            this.CreateMap<RawCommentServiceModel, Comment>()
                .ForMember(x => x.UserId, y => y.MapFrom(z => z.UserId))
                .ForMember(x => x.PostId, y => y.MapFrom(z => z.PostId))
                .ForMember(x => x.Content, y => y.MapFrom(z => z.CommentText));
        }
    }
}
