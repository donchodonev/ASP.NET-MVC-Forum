namespace ASP.NET_MVC_Forum.Services.Business.Post
{
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Services.Data.Post;
    using ASP.NET_MVC_Forum.Services.HtmlManipulator;
    using ASP.NET_MVC_Forum.Services.PostReport;
    using Microsoft.EntityFrameworkCore;
    using System;
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

            var postDescriptionWithoutHtml = htmlManipulator.Escape(decodedHtml);

            post.HtmlContent = decodedHtml;

            string postShortDescription;

            if (postDescriptionWithoutHtml.Length < 300)
            {
                postShortDescription = postDescriptionWithoutHtml.Substring(0, postDescriptionWithoutHtml.Length) + "...";
            }
            else
            {
                postShortDescription = postDescriptionWithoutHtml.Substring(0, 300) + "...";
            }

            post.ShortDescription = postShortDescription;

            var postId = await postDataService.AddPostAsync(post);

            reportService.AutoGeneratePostReport(post.Title, post.HtmlContent, postId);

            return postId;
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

            reportService.AutoGeneratePostReport(post.Title, post.HtmlContent, post.Id);
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
    }
}
