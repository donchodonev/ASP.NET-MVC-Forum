namespace ASP.NET_MVC_Forum.Business
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Enums;
    using ASP.NET_MVC_Forum.Domain.Models.Post;
    using ASP.NET_MVC_Forum.Infrastructure;
    using ASP.NET_MVC_Forum.Infrastructure.Extensions;
    using ASP.NET_MVC_Forum.Web.Services.Models.Post;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using Microsoft.EntityFrameworkCore;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class PostBusinessService : IPostBusinessService
    {
        private readonly IPostRepository postRepo;
        private readonly IPostReportBusinessService postReportBusinessService;
        private readonly IHtmlManipulator htmlManipulator;
        private readonly IUserDataService userService;
        private readonly IVoteRepository voteRepo;
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;

        public PostBusinessService(IPostRepository postRepo,
            IPostReportBusinessService postReportBusinessService,
            IHtmlManipulator htmlManipulator,
            IUserDataService userService,
            IVoteRepository voteRepo,
            ICategoryRepository categoryRepository,
            IMapper mapper)
        {
            this.postRepo = postRepo;
            this.postReportBusinessService = postReportBusinessService;
            this.htmlManipulator = htmlManipulator;
            this.userService = userService;
            this.voteRepo = voteRepo;
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }

        public async Task<NewlyCreatedPostServiceModel> CreateNewAsync(AddPostFormModel formModelPost, string identityUserId)
        {
            formModelPost.Categories = await categoryRepository.GetCategoryIdAndNameCombinationsAsync();

            var post = mapper.Map<Post>(formModelPost);

            post.UserId = await userService.GetBaseUserIdAsync(identityUserId);

            var sanitizedhtml = htmlManipulator.Sanitize(post.HtmlContent);

            var santizedAndDecodedHtml = htmlManipulator.Decode(sanitizedhtml);

            var santizedAndDecodedHtmlAndEscapedHtml = htmlManipulator.Escape(santizedAndDecodedHtml);

            post.HtmlContent = santizedAndDecodedHtmlAndEscapedHtml;

            post.ShortDescription = GenerateShortDescription(santizedAndDecodedHtmlAndEscapedHtml);

            await postRepo.AddPostAsync(post);

            await postReportBusinessService
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

            var postChanges = await GetPostChangesAsync(
                viewModelData.PostId,
                viewModelData.HtmlContent,
                viewModelData.Title,
                viewModelData.CategoryId);

            if (postChanges.Count == 0)
            {
                return originalPost;
            }

            AddPostChanges(originalPost, viewModelData, postChanges);

            var sanitizedHtml = htmlManipulator.Sanitize(originalPost.HtmlContent);

            var decodedHtml = htmlManipulator.Decode(sanitizedHtml);

            var postDescriptionWithoutHtmlTags = htmlManipulator.Escape(decodedHtml);

            originalPost.HtmlContent = decodedHtml;

            originalPost.ShortDescription = GeneratePostShortDescription(postDescriptionWithoutHtmlTags, 300);

            originalPost.ModifiedOn = DateTime.UtcNow;

            await postRepo.UpdateAsync(originalPost);

            await postReportBusinessService.AutoGeneratePostReportAsync(originalPost.Title, originalPost.HtmlContent, originalPost.Id);

            return originalPost;
        }

        public async Task<Dictionary<string, bool>> GetPostChangesAsync(int originalPostId, string newHtmlContent, string newTitle, int newCategoryId)
        {
            var originalPost = await postRepo.GetByIdAsync(originalPostId);

            var kvp = new Dictionary<string, bool>();

            var sanitizedAndDecodedHtml = htmlManipulator
                .Decode(htmlManipulator.Sanitize(newHtmlContent));

            if (originalPost.HtmlContent.Length != sanitizedAndDecodedHtml.Length)
            {
                kvp.Add("HtmlContent", true);
            }

            if (originalPost.Title != newTitle)
            {
                kvp.Add("Title", true);
            }

            if (originalPost.CategoryId != newCategoryId)
            {
                kvp.Add("CategoryId", true);
            }

            return kvp;
        }

        public Task<bool> IsAuthor(int userId, int postId)
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
            var posts = postRepo
                .All()
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Include(x => x.User)
                .ThenInclude(x => x.IdentityUser)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrWhiteSpace(searchTerm))
            {
                posts = posts.Where(post => post.Title.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(category) && !string.IsNullOrWhiteSpace(category) && category != "All")
            {
                posts = posts.Where(post => post.Category.Name == category);
            }

            if (sortOrder == 0 && sortType == 0)
            {
                posts = posts.OrderByDescending(x => x.CreatedOn);
            }
            else if (sortOrder == 0 && sortType == 1)
            {
                posts = posts.OrderByDescending(x => x.Title);
            }
            else if (sortOrder == 1 && sortType == 0)
            {
                posts = posts.OrderBy(x => x.CreatedOn);
            }
            else if (sortOrder == 1 && sortType == 1)
            {
                posts = posts.OrderBy(x => x.Title);
            }

            return posts.ProjectTo<PostPreviewViewModel>(mapper.ConfigurationProvider); ;
        }

        public Task<ViewPostViewModel> GenerateViewPostModelAsync(int postId)
        {
            var post = postRepo
                .GetById(postId)
                .Include(x => x.Comments)
                .Include(x => x.Votes)
                .Include(x => x.User).ThenInclude(x => x.Posts)
                .Include(x => x.User).ThenInclude(x => x.IdentityUser);

            return mapper
                .ProjectTo<ViewPostViewModel>(post)
                .FirstOrDefaultAsync();
        }

        public async Task<AddPostFormModel> GeneratedAddPostFormModelAsync()
        {
            var vm = new AddPostFormModel();

            vm.Categories = await categoryRepository.GetCategoryIdAndNameCombinationsAsync();

            return vm;
        }

        public async Task InjectUserLastVoteType(ViewPostViewModel viewModel, string identityUserId)
        {
            var baseUserId = await userService
                .GetBaseUserIdAsync(identityUserId);

            var vote = await voteRepo.GetUserVoteAsync(baseUserId, viewModel.PostId);

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

        private void AddPostChanges(Post originalPost, EditPostFormModel newPostData, Dictionary<string, bool> postChanges)
        {
            foreach (var kvp in postChanges)
            {
                if (kvp.Key == "HtmlContent")
                {
                    originalPost.HtmlContent = newPostData.HtmlContent;
                }
                if (kvp.Key == "Title")
                {
                    originalPost.Title = newPostData.Title;
                }
                if (kvp.Key == "CategoryId")
                {
                    originalPost.CategoryId = newPostData.CategoryId;
                }
            }
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
            var userId = await userService.GetBaseUserIdAsync(currentPrincipal.Id());

            return await IsAuthor(userId, postId) || currentPrincipal.IsAdminOrModerator();
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
