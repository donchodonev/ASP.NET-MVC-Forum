namespace ASP.NET_MVC_Forum.Infrastructure
{
    using ASP.NET_MVC_Forum.Areas.Admin.Models.CommentReport;
    using ASP.NET_MVC_Forum.Areas.Admin.Models.PostReport;
    using ASP.NET_MVC_Forum.Areas.Admin.Models.User;
    using ASP.NET_MVC_Forum.Areas.API.Models.Comments;
    using ASP.NET_MVC_Forum.Areas.API.Models.Stats;
    using ASP.NET_MVC_Forum.Areas.API.Models.Votes;
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Models.Chat;
    using ASP.NET_MVC_Forum.Models.Post;
    using ASP.NET_MVC_Forum.Services.Comment.Models;
    using ASP.NET_MVC_Forum.Services.Models.Post;
    using AutoMapper;
    using System.Linq;
    using static ASP.NET_MVC_Forum.Data.Constants.DateTimeConstants;
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Post, NewlyCreatedPostServiceModel>()
                .ForMember(x => x.Id, cfg => cfg.MapFrom(z => z.Id))
                .ForMember(x => x.Title, cfg => cfg.MapFrom(z => z.Title));

            this.CreateMap<Message, ChatMessageResponseData>()
                .ForMember(x => x.SenderUsername, y => y.MapFrom(y => y.SenderUsername))
                .ForMember(x => x.Time, y => y.MapFrom(y => y.CreatedOn.AddHours(2) // FOR GMT+2
                .ToString(DateAndTimeFormat)));

            this.CreateMap<User, ChatSelectUserViewModel>()
                .ForMember(x => x.RecipientUsername, cfg => cfg.MapFrom(y => y.IdentityUser.UserName))
                .ForMember(x => x.RecipientIdentityUserId, cfg => cfg.MapFrom(y => y.IdentityUser.Id));

            this.CreateMap<Category, MostPostsPerCategoryResponseModel>()
                .ForMember(x => x.Count, y => y.MapFrom(y => y.Posts.Count))
                .ForMember(x => x.Title, y => y.MapFrom(y => y.Name));

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

            this.CreateMap<User, UserViewModel>();

            this.CreateMap<PostReport, PostReportViewModel>();

            this.CreateMap<CommentPostRequestModel, RawCommentServiceModel>();

            this.CreateMap<Category, CategoryIdAndNameViewModel>();

            this.CreateMap<Comment, CommentGetRequestResponseModel>()
                .ForMember(x => x.CommentAuthor, y => y.MapFrom(z => z.User.IdentityUser.UserName));

            this.CreateMap<AddPostFormModel, Post>().ReverseMap();

            this.CreateMap<Post, EditPostFormModel>()
                .ForMember(x => x.PostId, y => y.MapFrom(z => z.Id))
                .ReverseMap();

            this.CreateMap<Post, PostPreviewViewModel>()
                .ForMember(x => x.UserPostsCount, y => y.MapFrom(z => z.User.Posts.Where(x => !x.IsDeleted).Count()))
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
