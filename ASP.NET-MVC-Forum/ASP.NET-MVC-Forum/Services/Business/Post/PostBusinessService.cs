namespace ASP.NET_MVC_Forum.Services.Business.Post
{
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Models.Post;
    using ASP.NET_MVC_Forum.Services.Business.HtmlManipulator;
    using ASP.NET_MVC_Forum.Services.Business.PostReport;
    using ASP.NET_MVC_Forum.Services.Data.Post;
    using ASP.NET_MVC_Forum.Services.Data.Vote;
    using ASP.NET_MVC_Forum.Services.User;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class PostBusinessService : IPostBusinessService
    {
        private readonly IPostDataService postDataService;
        private readonly IPostReportBusinessService postReportBusinessService;
        private readonly IHtmlManipulator htmlManipulator;
        private readonly IUserService userService;
        private readonly IVoteDataService voteDataService;
        private readonly IMapper mapper;

        public PostBusinessService(IPostDataService postDataService,
            IPostReportBusinessService postReportBusinessService,
            IHtmlManipulator htmlManipulator,
            IUserService userService,
            IVoteDataService voteDataService,
            IMapper mapper)
        {
            this.postDataService = postDataService;
            this.postReportBusinessService = postReportBusinessService;
            this.htmlManipulator = htmlManipulator;
            this.userService = userService;
            this.voteDataService = voteDataService;
            this.mapper = mapper;
        }

        public async Task<int> CreateNew(Post post, int userId)
        {
            post.UserId = userId;

            var sanitizedhtml = htmlManipulator.Sanitize(post.HtmlContent);
            var decodedHtml = htmlManipulator.Decode(sanitizedhtml);
            var escapedHtml = htmlManipulator.Escape(decodedHtml);

            post.HtmlContent = decodedHtml;

            post.ShortDescription = GenerateShortDescription(escapedHtml);

            var postId = await postDataService.AddPostAsync(post);

            return postId;
        }

        public string GenerateShortDescription(string escapedHtml)
        {
            string postShortDescription;

            if (escapedHtml.Length < 300)
            {
                postShortDescription = escapedHtml.Substring(0, escapedHtml.Length) + "...";
            }
            else
            {
                postShortDescription = escapedHtml.Substring(0, 300) + "...";
            }

            return postShortDescription;
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

        public async Task Edit(Post post)
        {
            var sanitizedHtml = htmlManipulator.Sanitize(post.HtmlContent);
            var decodedHtml = htmlManipulator.Decode(sanitizedHtml);

            var pattern = @"<.*?>";
            var replacement = string.Empty;

            var postDescriptionWithoutHtml = Regex.Replace(decodedHtml, pattern, replacement);

            string postShortDescription;

            if (postDescriptionWithoutHtml.Length < 300)
            {
                postShortDescription = postDescriptionWithoutHtml.Substring(0, postDescriptionWithoutHtml.Length) + "...";
            }
            else
            {
                postShortDescription = postDescriptionWithoutHtml.Substring(0, 300) + "...";
            }

            post.HtmlContent = decodedHtml;
            post.ShortDescription = postShortDescription;
            post.ModifiedOn = DateTime.UtcNow;

            await postDataService.UpdatePostAsync(post);

            await postReportBusinessService.AutoGeneratePostReportAsync(post.Title, post.HtmlContent, post.Id);
        }

        /// <summary>
        /// Checks which parts of a post have been changed during edit (if any)
        /// </summary>
        /// <param name="originalPost">The source post</param>
        /// <param name="newHtmlContent">The new post html content</param>
        /// <param name="newTitle">The new post title</param>
        /// <param name="newCategoryId">The new post category Id</param>
        /// <returns>Task<Dictionary<string, bool>></returns>

        public Dictionary<string, bool> GetPostChanges(Post originalPost, string newHtmlContent, string newTitle, int newCategoryId)
        {
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
    }
}
