﻿namespace ASP.NET_MVC_Forum.Business
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Business.Contracts.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Data.QueryBuilders;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Post;
    using ASP.NET_MVC_Forum.Infrastructure;
    using ASP.NET_MVC_Forum.Infrastructure.Extensions;
    using ASP.NET_MVC_Forum.Web.Services.Models.Post;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using Microsoft.EntityFrameworkCore;

    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class PostService : IPostService
    {
        private readonly IPostRepository postRepo;
        private readonly IPostReportService postReportService;
        private readonly IHtmlManipulator htmlManipulator;
        private readonly IVoteRepository voteRepo;
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;
        private readonly IPostValidationService postValidationService;

        public PostService(IPostRepository postRepo,
            IPostReportService postReportService,
            IHtmlManipulator htmlManipulator,
            IVoteRepository voteRepo,
            ICategoryRepository categoryRepository,
            IMapper mapper,
            IPostValidationService postValidationService)
        {
            this.postRepo = postRepo;
            this.postReportService = postReportService;
            this.htmlManipulator = htmlManipulator;
            this.voteRepo = voteRepo;
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
            this.postValidationService = postValidationService;
        }

        public async Task<NewlyCreatedPostServiceModel> CreateNewAsync(AddPostFormModel formModelPost)
        {
            formModelPost.Categories = await categoryRepository.GetCategoryIdAndNameCombinationsAsync();

            var post = mapper.Map<Post>(formModelPost);

            var sanitizedhtml = htmlManipulator.Sanitize(post.HtmlContent);

            var santizedAndDecodedHtml = htmlManipulator.Decode(sanitizedhtml);

            var santizedAndDecodedHtmlAndEscapedHtml = htmlManipulator.Escape(santizedAndDecodedHtml);

            post.HtmlContent = santizedAndDecodedHtmlAndEscapedHtml;

            post.ShortDescription = GenerateShortDescription(santizedAndDecodedHtmlAndEscapedHtml);

            await postRepo.AddPostAsync(post);

            await postReportService
                .AutoGeneratePostReportAsync(post.Title, post.HtmlContent, post.Id);

            return mapper.Map<NewlyCreatedPostServiceModel>(post);
        }

        public string GenerateShortDescription(string escapedHtml)
        {
            if (escapedHtml.Length < 300)
            {
                return escapedHtml.Substring(0, escapedHtml.Length) + "...";
            }

            return escapedHtml.Substring(0, 300) + "...";
        }

        /// <summary>
        /// Deletes post by post id
        /// </summary>
        /// <param name="postId">Post's Id</param>
        /// <returns>Task</returns>
        public async Task Delete(int postId)
        {
            var currentTime = DateTime.UtcNow;

            var postToMarkAsDeleted = await postRepo
                .GetById(postId)
                .Include(x => x.Reports)
                .FirstOrDefaultAsync();

            postToMarkAsDeleted.IsDeleted = true;

            postToMarkAsDeleted.ModifiedOn = currentTime;

            foreach (var report in postToMarkAsDeleted.Reports)
            {
                report.IsDeleted = true;
                report.ModifiedOn = currentTime;
            }

            await postRepo.UpdateAsync(postToMarkAsDeleted);
        }

        public async Task<Post> Edit(EditPostFormModel viewModelData)
        {
            var originalPost = await postRepo.GetByIdAsync(viewModelData.PostId);

            await postValidationService.ValidatePostChangedAsync(
                viewModelData.PostId,
                viewModelData.HtmlContent,
                viewModelData.Title,
                viewModelData.CategoryId);

            var sanitizedHtml = htmlManipulator.Sanitize(originalPost.HtmlContent);

            var decodedHtml = htmlManipulator.Decode(sanitizedHtml);

            var postDescriptionWithoutHtmlTags = htmlManipulator.Escape(decodedHtml);

            originalPost.HtmlContent = decodedHtml;

            originalPost.CategoryId = viewModelData.CategoryId;

            originalPost.Title = viewModelData.Title;

            originalPost.ShortDescription = GeneratePostShortDescription(postDescriptionWithoutHtmlTags, 300);

            originalPost.ModifiedOn = DateTime.UtcNow;

            await postRepo.UpdateAsync(originalPost);

            await postReportService.AutoGeneratePostReportAsync(originalPost.Title, originalPost.HtmlContent, originalPost.Id);

            return originalPost;
        }

        public Task<bool> IsAuthor(string userId, int postId)
        {
            return postRepo
                .All()
                .AsNoTracking()
                .AnyAsync(x => x.Id == postId && x.UserId == userId);
        }

        public IQueryable<PostPreviewViewModel> GetAllPostsSortedBy(
            int sortType,
            int sortOrder,
            string searchTerm,
            string category)
        {
            var allPosts = postRepo.All();

            var posts = new PostQueryBuilder(allPosts)
                .WithoutDeleted()
                .IncludeUser()
                .FindBySearchTerm(searchTerm)
                .FindByCategoryName(category)
                .Order(sortType, sortOrder)
                .BuildQuery();

            return posts.ProjectTo<PostPreviewViewModel>(mapper.ConfigurationProvider); ;
        }

        public Task<ViewPostViewModel> GenerateViewPostModelAsync(int postId)
        {
            var posts = postRepo
                .GetById(postId)
                .Include(x => x.Comments)
                .Include(x => x.Votes)
                .Include(x => x.User)
                .ThenInclude(x => x.Posts);

            var post = mapper
                .ProjectTo<ViewPostViewModel>(posts)
                .FirstOrDefaultAsync();

            postValidationService.ValidatePostModelNotNull(post);

            return post;
        }

        public async Task<AddPostFormModel> GeneratedAddPostFormModelAsync()
        {
            var vm = new AddPostFormModel();

            vm.Categories = await categoryRepository.GetCategoryIdAndNameCombinationsAsync();

            return vm;
        }

        public async Task InjectUserLastVoteType(ViewPostViewModel viewModel, string identityUserId)
        {
            var vote = await voteRepo.GetUserVoteAsync(identityUserId, viewModel.PostId);

            if (vote == null)
            {
                viewModel.UserLastVoteChoice = 0;
            }
            else
            {
                viewModel.UserLastVoteChoice = (int)vote.VoteType;
            }
        }

        public async Task<EditPostFormModel> GenerateEditPostFormModelAsync(int postId)
        {
            var post = postRepo
                        .GetById(postId)
                        .Include(x => x.Category);

            var vm = mapper
                    .ProjectTo<EditPostFormModel>(post)
                    .First();

            vm.Categories = await categoryRepository.GetCategoryIdAndNameCombinationsAsync();

            return vm;
        }

        public async Task<bool> PostExistsAsync(string postTitle)
        {
            return await postRepo.ExistsAsync(postTitle);
        }

        public async Task<bool> PostExistsAsync(int postId)
        {
            return await postRepo.ExistsAsync(postId);
        }

        public async Task<bool> IsUserPrivileged(int postId, ClaimsPrincipal currentPrincipal)
        {
            return await IsAuthor(currentPrincipal.Id(), postId) || currentPrincipal.IsAdminOrModerator();
        }

        private string GeneratePostShortDescription(string postDescriptionWithoutHtmlTags, int postDescriptionMaxLength)
        {
            if (postDescriptionWithoutHtmlTags.Length < postDescriptionMaxLength)
            {
                return postDescriptionWithoutHtmlTags.Substring(0, postDescriptionWithoutHtmlTags.Length) + "...";
            }

            return postDescriptionWithoutHtmlTags.Substring(0, postDescriptionMaxLength) + "...";
        }
    }
}
