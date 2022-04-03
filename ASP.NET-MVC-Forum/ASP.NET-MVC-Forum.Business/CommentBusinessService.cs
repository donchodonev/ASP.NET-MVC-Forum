﻿namespace ASP.NET_MVC_Forum.Business
{
    using ASP.NET_MVC_Forum.Business.Contracts;
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

    public class CommentBusinessService : ICommentBusinessService
    {
        private readonly ICommentRepository commentRepo;
        private readonly IMapper mapper;
        private readonly IUserRepository userRepo;
        private readonly ICommentReportBusinessService commentReportService;

        public CommentBusinessService(ICommentRepository commentRepo, IMapper mapper, IUserRepository userRepo, ICommentReportBusinessService commentReportService)
        {
            this.commentRepo = commentRepo;
            this.mapper = mapper;
            this.userRepo = userRepo;
            this.commentReportService = commentReportService;
        }

        public async Task<IEnumerable<CommentGetRequestResponseModel>> GenerateCommentGetRequestResponseModel(int postId)
        {
            var allCommentsQuery = commentRepo.All();

            var commentById = new CommentQueryBuilder(allCommentsQuery)
                .IncludeUser()
                .BuildQuery()
                .Where(x => x.PostId == postId);

            return await mapper
                .ProjectTo<CommentGetRequestResponseModel>(commentById)
                .OrderByDescending(x => x.CreatedOn)
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

        public async Task DeleteAsync(int commentId)
        {
            var comment = await commentRepo
                .GetAllById(commentId)
                .FirstAsync();

            comment.IsDeleted = true;

            await commentRepo.UpdateAsync(comment);
        }
    }
}
