namespace ASP.NET_MVC_Forum.Infrastructure.MappingProfiles
{
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Post;
    using ASP.NET_MVC_Forum.Domain.Models.Stats;
    using ASP.NET_MVC_Forum.Web.Services.Models.Post;

    using AutoMapper;

    using System.Linq;

    using static ASP.NET_MVC_Forum.Domain.Constants.DateTimeConstants;
    public class PostMappingProfile : Profile
    {
        public PostMappingProfile()
        {
            CreateMap<AddPostFormModel, Post>().ReverseMap();

            CreateMap<Post, NewlyCreatedPostServiceModel>()
                .ForMember(x => x.Id, cfg => cfg.MapFrom(z => z.Id))
                .ForMember(x => x.Title, cfg => cfg.MapFrom(z => z.Title));

            CreateMap<Post, MostReportedPostsResponeModel>()
                .ForMember(x => x.Count, y => y.MapFrom(y => y.Reports.Count));

            CreateMap<Post, MostLikedPostsResponeModel>()
                .ForMember(x => x.Count, y => y.MapFrom(y => y.Votes.Sum(x => (int)x.VoteType)));

            CreateMap<Post, MostCommentedPostsResponeModel>()
                .ForMember(x => x.Count, y => y.MapFrom(y => y.Comments.Count));

            CreateMap<Post, EditPostFormModel>()
                .ForMember(x => x.PostId, y => y.MapFrom(z => z.Id))
                .ReverseMap();

            CreateMap<Post, PostPreviewViewModel>()
                .ForMember(x => x.UserPostsCount, y => y.MapFrom(z => z.User.Posts.Where(x => !x.IsDeleted).Count()))
                .ForMember(x => x.UserIdentityUserUsername, y => y.MapFrom(z => z.User.UserName))
                .ForMember(x => x.UserImageUrl, y => y.MapFrom(z => z.User.ImageUrl))
                .ForMember(x => x.UserMemberSince, y => y.MapFrom(z => z.User.CreatedOn.ToString(DateFormat)))
                .ForMember(x => x.UserUsername, y => y.MapFrom(z => z.User.UserName))
                .ForMember(x => x.PostCreationDate, y => y.MapFrom(z => z.CreatedOn.ToString(DateAndTimeFormat) + " UTC"))
                .ForMember(x => x.PostId, y => y.MapFrom(z => z.Id));

            CreateMap<Post, ViewPostViewModel>()
                .ForMember(x => x.UserPostsCount, y => y.MapFrom(z => z.User.Posts.Count))
                .ForMember(x => x.UserIdentityUserUsername, y => y.MapFrom(z => z.User.UserName))
                .ForMember(x => x.UserImageUrl, y => y.MapFrom(z => z.User.ImageUrl))
                .ForMember(x => x.UserMemberSince, y => y.MapFrom(z => z.User.CreatedOn.ToString(DateFormat)))
                .ForMember(x => x.UserUsername, y => y.MapFrom(z => z.User.UserName))
                .ForMember(x => x.PostCreationDate, y => y.MapFrom(z => z.CreatedOn.ToString(DateAndTimeFormat) + " UTC"))
                .ForMember(x => x.CommentsCount, y => y.MapFrom(z => z.Comments.Where(x => x.IsDeleted == false).Count()))
                .ForMember(x => x.PostId, y => y.MapFrom(z => z.Id))
                .ForMember(x => x.VoteSum, y => y.MapFrom(z => z.Votes.Sum(x => (int)x.VoteType)));
        }
    }
}
