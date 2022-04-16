namespace ASP.NET_MVC_Forum.Business
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Models.Comment;
    using ASP.NET_MVC_Forum.Infrastructure.Extensions;
    using ASP.NET_MVC_Forum.Validation.Contracts;

    using AutoMapper;

    using Microsoft.EntityFrameworkCore;

    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class CommentService : ICommentService
    {
        private readonly ICommentRepository commentRepo;
        private readonly IMapper mapper;
        private readonly IPostValidationService postValidationService;
        private readonly ICommentReportService commentReportService;
        private readonly ICommentValidationService commentValidationService;

        public CommentService(
            ICommentRepository commentRepo,
            IMapper mapper,
            IPostValidationService postValidationService, 
            ICommentReportService commentReportService,
            ICommentValidationService commentValidationService)
        {
            this.commentRepo = commentRepo;
            this.mapper = mapper;
            this.postValidationService = postValidationService;
            this.commentReportService = commentReportService;
            this.commentValidationService = commentValidationService;
        }

        public async Task<List<CommentGetRequestResponseModel>> GenerateCommentGetResponseModelAsync(int postId)
        {
            await postValidationService.ValidatePostExistsAsync(postId);

            var commentsById = commentRepo
                .All()
                .Where(x => x.PostId == postId)
                .Include(x => x.User)
                .OrderByDescending(x => x.CreatedOn);

            return await mapper
                .ProjectTo<CommentGetRequestResponseModel>(commentsById)
                .ToListAsync();
        }

        public async Task<CommentPostResponseModel> GenerateCommentPostResponseModelAsync(
            CommentPostRequestModel commentData,
            string userId,
            string userUsername,
            int commentId)
        {
            var rawCommentData = mapper.Map<CommentPostResponseModel>(commentData);

            rawCommentData.UserId = userId;

            rawCommentData.Username = userUsername;

            rawCommentData.Id = commentId;

            await commentReportService.AutoGenerateCommentReportAsync(rawCommentData.CommentText, rawCommentData.Id);

            return rawCommentData;
        }

        public async Task DeleteAsync(int commentId, ClaimsPrincipal user)
        {
            var comment = await commentRepo
                .GetById(commentId)
                .FirstAsync();

            commentValidationService.ValidateCommentNotNull(comment);

            await commentValidationService.ValidateUserCanDeleteCommentAsync(commentId, user);

            comment.IsDeleted = true;

            await commentRepo.UpdateAsync(comment);
        }
    }
}
