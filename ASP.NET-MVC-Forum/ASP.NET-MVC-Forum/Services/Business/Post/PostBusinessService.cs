namespace ASP.NET_MVC_Forum.Services.Business.Post
{
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Services.Data.Post;
    using ASP.NET_MVC_Forum.Services.HtmlManipulator;
    using ASP.NET_MVC_Forum.Services.PostReport;
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class PostBusinessService : IPostBusinessService
    {
        private readonly IPostDataService postDataService;
        private readonly IPostReportService reportService;
        private readonly IHtmlManipulator htmlManipulator;

        public PostBusinessService(IPostDataService postDataService,IPostReportService reportService, IHtmlManipulator htmlManipulator)
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
    }
}
