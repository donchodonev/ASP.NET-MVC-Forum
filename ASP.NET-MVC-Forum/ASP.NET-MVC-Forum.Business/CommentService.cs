namespace ASP.NET_MVC_Forum.Business
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Business.Contracts.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Data.QueryBuilders;
    using ASP.NET_MVC_Forum.Domain.Models.Comment;
    using ASP.NET_MVC_Forum.Infrastructure.Extensions;


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

        public async Task<IEnumerable<CommentGetRequestResponseModel>> GenerateCommentGetRequestResponseModel(int postId)
        {
            await postValidationService.ValidatePostExistsAsync(postId);

            var commentById = commentRepo
                .All()
                .Where(x => x.PostId == postId)
                .Include(x => x.User)
                .OrderByDescending(x => x.CreatedOn);

            return await mapper
                .ProjectTo<CommentGetRequestResponseModel>(commentById)
                .ToListAsync();
        }

        public async Task<RawCommentServiceModel> GenerateRawCommentServiceModel(CommentPostRequestModel commentData, ClaimsPrincipal user)
        {
            var rawCommentData = mapper.Map<RawCommentServiceModel>(commentData);

            rawCommentData.UserId = user.Id();

            rawCommentData.Username = user.Identity.Name;

            rawCommentData.Id = await commentRepo.AddCommentAsync(rawCommentData);

            await commentReportService.AutoGenerateCommentReportAsync(rawCommentData.CommentText, rawCommentData.Id);

            return rawCommentData;
        }

        public async Task<bool> CommentExistsAsync(int commentId)
        {
            return await commentRepo.All().AnyAsync(x => x.Id == commentId);
        }

        public Task<bool> IsUserPrivileged(int commentId, ClaimsPrincipal user)
        {
            string userId = user.Id();

            return commentRepo
                .All()
                .AnyAsync(x => x.Id == commentId &&
                (x.UserId == userId || user.IsAdminOrModerator()));
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
