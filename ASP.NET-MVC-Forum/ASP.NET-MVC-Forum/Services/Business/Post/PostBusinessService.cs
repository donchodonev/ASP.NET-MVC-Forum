namespace ASP.NET_MVC_Forum.Services.Business.Post
{
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Models.Post;
    using ASP.NET_MVC_Forum.Services.Business.HtmlManipulator;
    using ASP.NET_MVC_Forum.Services.Business.PostReport;
    using ASP.NET_MVC_Forum.Services.Data.Category;
    using ASP.NET_MVC_Forum.Services.Data.Post;
    using ASP.NET_MVC_Forum.Services.Data.Vote;
    using ASP.NET_MVC_Forum.Services.Models.Post;
    using ASP.NET_MVC_Forum.Services.Data.User;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Security.Claims;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ClaimsPrincipalExtensions;


    public class PostBusinessService : IPostBusinessService
    {
        private readonly IPostDataService postDataService;
        private readonly IPostReportBusinessService postReportBusinessService;
        private readonly IHtmlManipulator htmlManipulator;
        private readonly IUserDataService userService;
        private readonly IVoteDataService voteDataService;
        private readonly ICategoryDataService categoryService;
        private readonly IMapper mapper;

        public PostBusinessService(IPostDataService postDataService,
            IPostReportBusinessService postReportBusinessService,
            IHtmlManipulator htmlManipulator,
            IUserDataService userService,
            IVoteDataService voteDataService,
            ICategoryDataService categoryService,
            IMapper mapper)
        {
            this.postDataService = postDataService;
            this.postReportBusinessService = postReportBusinessService;
            this.htmlManipulator = htmlManipulator;
            this.userService = userService;
            this.voteDataService = voteDataService;
            this.categoryService = categoryService;
            this.mapper = mapper;
        }

        public async Task<NewlyCreatedPostServiceModel> CreateNewAsync(AddPostFormModel formModelPost, string identityUserId)
        {
            var post = mapper.Map<Post>(formModelPost);

            post.UserId = await userService.GetBaseUserIdAsync(identityUserId);

            var sanitizedhtml = htmlManipulator.Sanitize(post.HtmlContent);
            var santizedAndDecodedHtml = htmlManipulator.Decode(sanitizedhtml);
            var santizedAndDecodedHtmlAndEscapedHtml = htmlManipulator.Escape(santizedAndDecodedHtml);

            post.HtmlContent = santizedAndDecodedHtmlAndEscapedHtml;
            post.ShortDescription = GenerateShortDescription(santizedAndDecodedHtmlAndEscapedHtml);

            await postDataService.AddPostAsync(post);

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

            var postToMarkAsDeleted = await postDataService
                .GetByIdAsync(postId, PostQueryFilter.WithReports);

            postToMarkAsDeleted.IsDeleted = true;
            postToMarkAsDeleted.ModifiedOn = currentTime;

            foreach (var report in postToMarkAsDeleted.Reports)
            {
                report.IsDeleted = true;
                report.ModifiedOn = currentTime;
            }

            await postDataService.UpdatePostAsync(postToMarkAsDeleted);
        }

        public async Task<Post> Edit(EditPostFormModel viewModelData)
        {
            var originalPost = await postDataService
                .GetByIdAsync(viewModelData.PostId, PostQueryFilter.WithoutDeleted);

            var postChanges = await GetPostChangesAsync(viewModelData.PostId, viewModelData.HtmlContent, viewModelData.Title, viewModelData.CategoryId);

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

            await postDataService.UpdatePostAsync(originalPost);

            await postReportBusinessService.AutoGeneratePostReportAsync(originalPost.Title, originalPost.HtmlContent, originalPost.Id);

            return originalPost;
        }

        /// <summary>
        /// Checks which parts of a post have been changed during edit (if any)
        /// </summary>
        /// <param name="originalPost">The source post</param>
        /// <param name="newHtmlContent">The new post html content</param>
        /// <param name="newTitle">The new post title</param>
        /// <param name="newCategoryId">The new post category Id</param>
        /// <returns>Task<Dictionary<string, bool>></returns>

        public async Task<Dictionary<string, bool>> GetPostChangesAsync(int originalPostId, string newHtmlContent, string newTitle, int newCategoryId)
        {
            var originalPost = await postDataService.GetByIdAsync(originalPostId);

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

        public async Task<bool> IsAuthor(int userId, int postId)
        {
            var posts = postDataService.All(PostQueryFilter.AsNoTracking);

            return await posts.AnyAsync(x => x.Id == postId && x.UserId == userId);
        }

        public IQueryable<PostPreviewViewModel> GetAllPostsSortedBy(int sortType, int sortOrder, string searchTerm, string category)
        {
            var posts = postDataService.All(
                    PostQueryFilter.WithUser,
                    PostQueryFilter.WithIdentityUser,
                    PostQueryFilter.WithoutDeleted,
                    PostQueryFilter.AsNoTracking);

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

        public async Task<ViewPostViewModel> GenerateViewPostModel(int postId)
        {
            var post = await postDataService.GetByIdAsync(postId,
                PostQueryFilter.WithIdentityUser,
                PostQueryFilter.WithUserPosts,
                PostQueryFilter.WithComments,
                PostQueryFilter.WithVotes);

            return mapper.Map<ViewPostViewModel>(post) ?? null;
        }

        public AddPostFormModel GeneratedAddPostFormModel()
        {
            var vm = new AddPostFormModel();

            vm.FillCategories(categoryService);

            return vm;
        }

        public async Task InjectUserLastVoteType(ViewPostViewModel viewModel, string identityUserId)
        {
            var baseUserId = await userService
                .GetBaseUserIdAsync(identityUserId);

            var vote = await voteDataService.GetUserVoteAsync(baseUserId, viewModel.PostId);

            if (vote == null)
            {
                viewModel.UserLastVoteChoice = 0;
            }
            else
            {
                viewModel.UserLastVoteChoice = (int)vote.VoteType;
            }
        }

        public EditPostFormModel GenerateEditPostFormModel(int postId)
        {
            var post = postDataService
               .GetByIdAsQueryable(postId, PostQueryFilter.WithCategory);

            var vm = mapper
                .ProjectTo<EditPostFormModel>(post)
                .First();

            vm.FillCategories(categoryService);

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
            return await postDataService.PostExistsAsync(postTitle);
        }

        public async Task<bool> PostExistsAsync(int postId)
        {
            return await postDataService.PostExistsAsync(postId);
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
