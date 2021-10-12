namespace ASP.NET_MVC_Forum.Services.Post
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Models;
    using Ganss.XSS;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web;

    public class PostService : IPostService
    {
        private readonly ApplicationDbContext db;
        private readonly IHtmlSanitizer sanitizer;

        public PostService(ApplicationDbContext db, IHtmlSanitizer sanitizer)
        {
            this.db = db;
            this.sanitizer = sanitizer;
        }

        /// <summary>
        /// Returns all posts with or without their category included
        /// </summary>
        /// <param name="withCategory">True to include post's Category property, false for a null Category property</param>
        /// <returns></returns>
        public async Task<IQueryable<Post>> AllAsync(bool withCategoryIncluded = false, bool withUserIncluded = false)
        {
            return await Task.Run(() => 
            {
                var query = db.Posts.Where(x => x.IsDeleted == false);

                if (withCategoryIncluded)
                {
                    query = query.Include(x => x.Category);
                }

                if (withUserIncluded)
                {
                    query = query.Include(x => x.User);
                }

                return query;
            });
        }
        public async Task<int> AddPostAsync(Post post, int baseUserId)
        {
            post.UserId = baseUserId;

            var sanitizedHtml = HttpUtility.HtmlDecode(sanitizer.Sanitize(post.HtmlContent));

            var pattern = @"<.*?>";
            var replacement = string.Empty;

            var postDescriptionWithoutHtml = Regex.Replace(sanitizedHtml, pattern, replacement);

            string postShortDescription;

            if (postDescriptionWithoutHtml.Length < 300)
            {
                postShortDescription = postDescriptionWithoutHtml.Substring(0, postDescriptionWithoutHtml.Length) + "...";
            }
            else
            {
                postShortDescription = postDescriptionWithoutHtml.Substring(0, 300) + "...";
            }

            post.HtmlContent = sanitizedHtml;
            post.ShortDescription = postShortDescription;

            await db.Posts.AddAsync(post);

            await db.SaveChangesAsync();

            var postWithId = await db.Posts.FirstAsync(x => x == post);

            return postWithId.Id;
        }
    }
}
