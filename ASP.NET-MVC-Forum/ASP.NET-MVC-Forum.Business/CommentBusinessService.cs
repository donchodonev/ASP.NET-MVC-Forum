namespace ASP.NET_MVC_Forum.Business
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Infrastructure.Extensions;
    using ASP.NET_MVC_Forum.Web.Areas.API.Models.Comments;
    using ASP.NET_MVC_Forum.Web.Services.Comment.Models;

    using AutoMapper;

    using Microsoft.EntityFrameworkCore;

    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class CommentBusinessService : ICommentBusinessService
    {
        private readonly ICommentDataService data;
        private readonly IMapper mapper;
        private readonly IUserDataService users;
        private readonly ICommentReportBusinessService commentReportService;

        public CommentBusinessService(ICommentDataService data, IMapper mapper, IUserDataService users, ICommentReportBusinessService commentReportService)
        {
            this.data = data;
            this.mapper = mapper;
            this.users = users;
            this.commentReportService = commentReportService;
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

            await commentReportService.AutoGenerateCommentReportAsync(rawCommentData.CommentText, rawCommentData.Id);

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
