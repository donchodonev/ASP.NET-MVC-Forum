namespace ASP.NET_MVC_Forum.Business
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Business.Contracts.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Data.QueryBuilders;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Post;
    using ASP.NET_MVC_Forum.Infrastructure;
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

        public async Task<NewlyCreatedPostServiceModel> CreateNewAsync(AddPostFormModel post, string userId)
        {
            await postValidationService.ValidateTitleNotDuplicateAsync(post.Title);

            post.Categories = await categoryRepository.GetCategoryIdAndNameCombinationsAsync();

            var postEntity = mapper.Map<Post>(post);

            var sanitizedhtml = htmlManipulator.Sanitize(post.HtmlContent);

            var santizedAndDecodedHtml = htmlManipulator.Decode(sanitizedhtml);

            var santizedAndDecodedHtmlAndEscapedHtml = htmlManipulator.Escape(santizedAndDecodedHtml);

            post.HtmlContent = santizedAndDecodedHtmlAndEscapedHtml;

            postEntity.ShortDescription = GenerateShortDescription(santizedAndDecodedHtmlAndEscapedHtml);

            postEntity.UserId = userId;

            await postRepo.AddPostAsync(postEntity);

            await postReportService
                .AutoGeneratePostReportAsync(post.Title, post.HtmlContent, postEntity.Id);

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
        public async Task Delete(int postId, ClaimsPrincipal user)
        {
            await userValidationService.ValidateUserIsPrivilegedAsync(postId, user);

            var currentTime = DateTime.UtcNow;

            var postToMarkAsDeleted = await postRepo
                .GetById(postId)
                .Include(x => x.Reports)
                .FirstOrDefaultAsync();

            postValidationService.ValidatePostModelNotNull(postToMarkAsDeleted);

            postToMarkAsDeleted.IsDeleted = true;

            postToMarkAsDeleted.ModifiedOn = currentTime;

            foreach (var report in postToMarkAsDeleted.Reports)
            {
                report.IsDeleted = true;
                report.ModifiedOn = currentTime;
            }

            await postRepo.UpdateAsync(postToMarkAsDeleted);
        }

        public async Task<Post> Edit(EditPostFormModel viewModelData, ClaimsPrincipal user)
        {
            await userValidationService.ValidateUserIsPrivilegedAsync(viewModelData.PostId, user);

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

            return posts.ProjectTo<PostPreviewViewModel>(mapper.ConfigurationProvider);
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

        public async Task<EditPostFormModel> GenerateEditPostFormModelAsync(int postId, ClaimsPrincipal user)
        {
            await userValidationService.ValidateUserIsPrivilegedAsync(postId, user);

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
