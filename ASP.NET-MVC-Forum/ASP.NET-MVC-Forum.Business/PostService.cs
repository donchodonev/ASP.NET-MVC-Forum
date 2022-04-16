namespace ASP.NET_MVC_Forum.Business
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Data.QueryBuilders;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Post;
    using ASP.NET_MVC_Forum.Infrastructure;
    using ASP.NET_MVC_Forum.Validation.Contracts;
    using ASP.NET_MVC_Forum.Web.Services.Models.Post;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using Microsoft.EntityFrameworkCore;

    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        private readonly IUserValidationService userValidationService;

        public PostService(IPostRepository postRepo,
            IPostReportService postReportService,
            IHtmlManipulator htmlManipulator,
            IVoteRepository voteRepo,
            ICategoryRepository categoryRepository,
            IMapper mapper,
            IPostValidationService postValidationService,
            IUserValidationService userValidationService)
        {
            this.postRepo = postRepo;
            this.postReportService = postReportService;
            this.htmlManipulator = htmlManipulator;
            this.voteRepo = voteRepo;
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
            this.postValidationService = postValidationService;
            this.userValidationService = userValidationService;
        }

        public async Task<NewlyCreatedPostServiceModel> CreateNewAsync(
            AddPostFormModel postFormModel,
            string userId)
        {
            await postValidationService.ValidateTitleNotDuplicateAsync(postFormModel.Title);

            await userValidationService.ValidateUserExistsByIdAsync(userId);

            postFormModel.Categories = await categoryRepository.GetCategoryIdAndNameCombinationsAsync();

            var post = mapper.Map<Post>(postFormModel);

            postFormModel.HtmlContent = ProcessHtml(postFormModel.HtmlContent);

            post.ShortDescription = GenerateShortDescription(postFormModel.HtmlContent);

            post.UserId = userId;

            await postRepo.AddPostAsync(post);

            await postReportService.AutoGeneratePostReportAsync(
                postFormModel.Title,
                postFormModel.HtmlContent,
                post.Id);

            return mapper.Map<NewlyCreatedPostServiceModel>(post);
        }

        public async Task Delete(
            int postId,
            string userId,
            bool isUserAdminOrModerator)
        {
            var post = await postRepo
                .GetById(postId)
                .Include(x => x.Reports)
                .FirstOrDefaultAsync();

            postValidationService.ValidateNotNull(post);

            await userValidationService.ValidateUserIsPrivilegedAsync(
                postId,
                userId,
                isUserAdminOrModerator);

            var currentTime = DateTime.UtcNow;

            post.IsDeleted = true;

            post.ModifiedOn = currentTime;

            MarkReportsDeleted(post.Reports, currentTime);

            await postRepo.UpdateAsync(post);
        }

        public async Task<Post> Edit(
            EditPostFormModel viewModelData,
            string userId,
            bool isAdminOrModerator)
        {
            await userValidationService.ValidateUserIsPrivilegedAsync(
                viewModelData.PostId,
                userId,
                isAdminOrModerator);

            var originalPost = await postRepo.GetByIdAsync(viewModelData.PostId);

            postValidationService.ValidateNotNull(originalPost);

            await postValidationService.ValidatePostChangedAsync(
                viewModelData.PostId,
                viewModelData.HtmlContent,
                viewModelData.Title,
                viewModelData.CategoryId);

            originalPost.HtmlContent = ProcessHtml(viewModelData.HtmlContent);

            originalPost.CategoryId = viewModelData.CategoryId;

            originalPost.Title = viewModelData.Title;

            originalPost.ShortDescription = GeneratePostShortDescription(originalPost.HtmlContent, 300);

            originalPost.ModifiedOn = DateTime.UtcNow;

            await postRepo.UpdateAsync(originalPost);

            await postReportService.AutoGeneratePostReportAsync(originalPost.Title, originalPost.HtmlContent, originalPost.Id);

            return originalPost;
        }

        public IQueryable<PostPreviewViewModel> GeneratePostPreviewViewModel(
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

            return posts.ProjectTo<PostPreviewViewModel>(mapper.ConfigurationProvider);
        }

        public async Task<ViewPostViewModel> GenerateViewPostModelAsync(int postId)
        {
            var post = postRepo
                .GetById(postId)
                .Include(x => x.Comments)
                .Include(x => x.Votes)
                .Include(x => x.User)
                .ThenInclude(x => x.Posts);

            var model = await mapper
                .ProjectTo<ViewPostViewModel>(post)
                .FirstOrDefaultAsync();

            postValidationService.ValidateNotNull(model);

            return model;
        }

        public async Task<AddPostFormModel> GenerateAddPostFormModelAsync()
        {
            var vm = new AddPostFormModel();

            vm.Categories = await categoryRepository.GetCategoryIdAndNameCombinationsAsync();

            return vm;
        }

        public async Task InjectUserLastVoteType(ViewPostViewModel viewModel, string identityUserId)
        {
            await userValidationService.ValidateUserExistsByIdAsync(identityUserId);

            var vote = await voteRepo.GetUserVoteAsync(identityUserId, viewModel.PostId);

            viewModel.UserLastVoteChoice = vote switch
            {
                null => 0,
                _ => (int)vote.VoteType
            };
        }

        public async Task<EditPostFormModel> GenerateEditPostFormModelAsync(
            int postId,
            string userId,
            bool isAdminOrModerator)
        {
            await userValidationService.ValidateUserIsPrivilegedAsync(
                postId,
                userId,
                isAdminOrModerator);

            var post = postRepo
                        .GetById(postId)
                        .Include(x => x.Category);

            var vm = mapper
                    .ProjectTo<EditPostFormModel>(post)
                    .First();

            vm.Categories = await categoryRepository.GetCategoryIdAndNameCombinationsAsync();

            return vm;
        }

        private void MarkReportsDeleted(ICollection<PostReport> reports, DateTime currentTime)
        {
            foreach (var report in reports)
            {
                report.IsDeleted = true;
                report.ModifiedOn = currentTime;
            }
        }

        private string GenerateShortDescription(string escapedHtml)
        {
            if (escapedHtml.Length < 300)
            {
                return escapedHtml.Substring(0, escapedHtml.Length) + "...";
            }

            return escapedHtml.Substring(0, 300) + "...";
        }

        private string GeneratePostShortDescription(string postDescriptionWithoutHtmlTags, int postDescriptionMaxLength)
        {
            if (postDescriptionWithoutHtmlTags.Length < postDescriptionMaxLength)
            {
                return postDescriptionWithoutHtmlTags.Substring(0, postDescriptionWithoutHtmlTags.Length) + "...";
            }

            return postDescriptionWithoutHtmlTags.Substring(0, postDescriptionMaxLength) + "...";
        }

        private string ProcessHtml(string html)
        {
            var sanitizedhtml = htmlManipulator.Sanitize(html);
            var santizedAndDecodedHtml = htmlManipulator.Decode(sanitizedhtml);
            var santizedAndDecodedHtmlAndEscapedHtml = htmlManipulator.Escape(santizedAndDecodedHtml);

            return santizedAndDecodedHtmlAndEscapedHtml;
        }
    }
}
