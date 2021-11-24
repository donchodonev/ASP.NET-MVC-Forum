namespace ASP.NET_MVC_Forum.Services.Business.Censor
{
    using ASP.NET_MVC_Forum.Services.Data.Post;
    using ProfanityFilter.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class CensorService : ICensorService
    {
        private readonly IPostDataService posts;
        private readonly IProfanityFilter filter;

        public CensorService(IPostDataService posts, IProfanityFilter filter)
        {
            this.posts = posts;
            this.filter = filter;
        }
        public List<string> FindPostProfanities(string title, string content)
        {
            List<string> profaneWordsFound = filter
                .DetectAllProfanities(content)
                .ToList();

            profaneWordsFound
                .AddRange(filter.DetectAllProfanities(title));

            return profaneWordsFound;
        }

        public List<string> FindPostProfanities(string title, string content, string shortDescription)
        {
            List<string> profaneWordsFound = filter
                .DetectAllProfanities(content.Substring(3, content.Length - 3))
                .ToList();

            profaneWordsFound.AddRange(filter.DetectAllProfanities(title));
            profaneWordsFound.AddRange(filter.DetectAllProfanities(shortDescription));

            return profaneWordsFound;
        }

        public bool ContainsProfanity(string term)
        {
            return filter.ContainsProfanity(term);
        }

        public async Task CensorPostAsync(int postId)
        {
            var post = await posts.GetByIdAsync(postId);

            var title = filter.CensorString(post.Title, '*');
            var htmlContent = filter.CensorString(post.HtmlContent, '*');
            var shortDescription = filter.CensorString(post.ShortDescription, '*');

            post.Title = title;
            post.HtmlContent = htmlContent;
            post.ShortDescription = shortDescription;

            await posts.UpdatePostAsync(post);
        }

        public async Task CensorPostWithRegexAsync(int postId)
        {
            var post = await posts.GetByIdAsync(postId);

            var profanities = FindPostProfanities(post.Title, post.HtmlContent, post.ShortDescription);

            var title = post.Title;
            var htmlContent = post.HtmlContent;
            var shortDescription = post.ShortDescription;

            foreach (var profanity in profanities)
            {
                title = Regex.Replace(title, $"\\w*{profanity}\\w*", "*****");
                htmlContent = Regex.Replace(htmlContent, $"\\w*{profanity}\\w*", "*****");
                shortDescription = Regex.Replace(shortDescription, $"\\w*{profanity}\\w*", "*****");
            }

            post.Title = title;
            post.HtmlContent = htmlContent;
            post.ShortDescription = shortDescription;

            await posts.UpdatePostAsync(post);
        }
    }
}
