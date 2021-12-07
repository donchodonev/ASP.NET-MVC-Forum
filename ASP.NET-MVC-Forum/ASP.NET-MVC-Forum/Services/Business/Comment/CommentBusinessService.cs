namespace ASP.NET_MVC_Forum.Services.Business.Comment
{
    using ASP.NET_MVC_Forum.Areas.API.Models.Comments;
    using ASP.NET_MVC_Forum.Services.Comment.Models;
    using ASP.NET_MVC_Forum.Services.Data.Comment;
    using ASP.NET_MVC_Forum.Services.Data.User;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ClaimsPrincipalExtensions;

    public class CommentBusinessService : ICommentBusinessService
    {
        private readonly ICommentDataService data;
        private readonly IMapper mapper;
        private readonly IUserDataService users;

        public CommentBusinessService(ICommentDataService data, IMapper mapper, IUserDataService users)
        {
            this.data = data;
            this.mapper = mapper;
            this.users = users;
        }

        public async Task<IEnumerable<CommentGetRequestResponseModel>> GenerateCommentGetRequestResponseModel(int postId)
        {
            return await mapper
            .ProjectTo<CommentGetRequestResponseModel>(data.GetAllByPostId(postId, false, true, true))
            .OrderByDescending(x => x.CreatedOn)
            .ToListAsync();
        }

        public async Task<RawCommentServiceModel> GenerateRawCommentServiceModel(CommentPostRequestModel commentData, ClaimsPrincipal user)
        {
            var rawCommentData = mapper.Map<RawCommentServiceModel>(commentData);

            rawCommentData.UserId = await users.GetBaseUserIdAsync(user.Id());
            rawCommentData.Username = user.Identity.Name;
            rawCommentData.Id = await data.AddComment(rawCommentData);

            return rawCommentData;
        }

        public async Task<bool> CommentExistsAsync(int commentId)
        {
            return await data.All().AnyAsync(x => x.Id == commentId);
        }

        public async Task<bool> IsUserPrivileged(int commentId, ClaimsPrincipal user)
        {
            var baseUserId = await users.GetBaseUserIdAsync(user.Id());
            
            return await data
                .All()
                .AnyAsync(x => x.Id == commentId &&
                (x.UserId == baseUserId || user.IsAdminOrModerator()));
        }

        public async Task DeleteAsync(int commentId)
        {
            var comment = await data.GetById(commentId).FirstAsync();

            comment.IsDeleted = true;

            await data.UpdateAsync(comment);
        }
    }
}
