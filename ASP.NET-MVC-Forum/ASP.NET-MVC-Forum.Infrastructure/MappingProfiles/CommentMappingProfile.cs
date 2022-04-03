namespace ASP.NET_MVC_Forum.Infrastructure.MappingProfiles
{
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Comment;

    using AutoMapper;

    public class CommentMappingProfile : Profile
    {
        public CommentMappingProfile()
        {
            CreateMap<CommentPostRequestModel, RawCommentServiceModel>();

            CreateMap<Comment, CommentGetRequestResponseModel>()
            .ForMember(x => x.CommentAuthor, y => y.MapFrom(z => z.User.UserName));

            CreateMap<RawCommentServiceModel, Comment>()
            .ForMember(x => x.UserId, y => y.MapFrom(z => z.UserId))
            .ForMember(x => x.PostId, y => y.MapFrom(z => z.PostId))
            .ForMember(x => x.Content, y => y.MapFrom(z => z.CommentText));
        }
    }
}
