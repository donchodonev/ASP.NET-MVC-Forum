namespace ASP.NET_MVC_Forum.Services.Business.Post
{
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Services.Data.Post;
    using ASP.NET_MVC_Forum.Services.HtmlManipulator;
    using ASP.NET_MVC_Forum.Services.PostReport;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ClaimsPrincipalExtensions;

    public class PostBusinessService : IPostBusinessService
    {
        private readonly IPostDataService postDataService;
        private readonly IPostReportService reportService;
        private readonly IHtmlManipulator htmlManipulator;

        public PostBusinessService(IPostDataService postDataService, IPostReportService reportService, IHtmlManipulator htmlManipulator)
        {
            this.postDataService = postDataService;
            this.reportService = reportService;
            this.htmlManipulator = htmlManipulator;
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
                .GetByIdAsync(postId,PostQueryFilter.WithReports);

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

            await reportService .AutoGeneratePostReport(post.Title, post.HtmlContent, post.Id);
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

        public async Task<bool> UserCanEdit(int userId, int postId, ClaimsPrincipal principal)
        {
            var isAuthor = await IsAuthor(userId,postId);
            var isPrivileged = principal.IsAdminOrModerator();

            return isAuthor || isPrivileged;
        }

        public IQueryable<Post> SortAndOrder(IQueryable<Post> posts, int sortType, int sortOrder, string searchTerm, string category)
        {
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

            return posts;
        }
    }
}
