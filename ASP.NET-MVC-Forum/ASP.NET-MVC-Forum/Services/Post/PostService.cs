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
        /// Returns all posts
        /// </summary>
        /// <param name="withCategoryIncluded">True to include post's Category property, false for a null Category property</param>
        /// <param name="withUserIncluded">True to include post's User property, false for a null User property</param>
        /// <param name="withIdentityUserIncluded">True to include post's IdentityUser property, false for a null IdentityUser property</param>
        /// <returns></returns>
        public async Task<IQueryable<Post>> AllAsync(bool withCategoryIncluded = false, bool withUserIncluded = false, bool withIdentityUserIncluded = false)
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

                if (withIdentityUserIncluded)
                {
                    query = query.Include(x => x.User.IdentityUser);
                }

                return query;
            });
        }


        /// <summary>
        /// Returns all posts with AsNoTracking for better performance. Only use with read-only operations
        /// </summary>
        /// <param name="withCategoryIncluded">True to include post's Category property, false for a null Category property</param>
        /// <param name="withUserIncluded">True to include post's User property, false for a null User property</param>
        /// <param name="withIdentityUserIncluded">True to include post's IdentityUser property, false for a null IdentityUser property</param>
        /// <returns></returns>
        public async Task<IQueryable<Post>> AllAsNoTrackingAsync(bool withCategoryIncluded = false, bool withUserIncluded = false, bool withIdentityUserIncluded = false)
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

                if (withIdentityUserIncluded)
                {
                    query = query.Include(x => x.User.IdentityUser);
                }

                return query.AsNoTracking();
            });
        }

        /// <summary>
        /// Adds a new post to the database
        /// </summary>
        /// <param name="post">Object of type Post</param>
        /// <param name="userId">Post's author user id</param>
        /// <returns>The newly added post's Id</returns>
        public async Task<int> AddPostAsync(Post post, int userId)
        {
            post.UserId = userId;

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

        /// <summary>
        /// Returns a Post object with the given Id
        /// </summary>
        /// <param name="postId">Post id</param>
        /// <param name="withCategoryIncluded">True for Post's Category property to be included, false for null</param>
        /// <param name="withUserIncluded">True for Post's User property to be included, false for null</param>
        /// <param name="withIdentityUserIncluded">True for Post's User.IdentityUser property to be included, false for null</param>
        /// <returns></returns>
        public async Task<Post> GetByIdAsync(int postId, bool withCategoryIncluded = false, bool withUserIncluded = false, bool withIdentityUserIncluded = false)
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

            if (withIdentityUserIncluded)
            {
                query = query.Include(x => x.User.IdentityUser);
            }

            return await query.FirstOrDefaultAsync(x => x.Id == postId);
        }

        public async Task<IQueryable<Post>> GetByCategoryAsync(int categoryId, bool withUserIncluded = false, bool withIdentityUserIncluded = false)
        {
            return await Task.Run(() =>
            {
                var query = db.Posts.Where(x => x.IsDeleted == false && x.CategoryId == categoryId);

                if (withUserIncluded)
                {
                    query = query.Include(x => x.User);
                }

                if (withIdentityUserIncluded)
                {
                    query = query.Include(x => x.User.IdentityUser);
                }

                return query;
            });
        }

        public async Task<bool> PostExistsAsync(string postTitle)
        {
            return await db.Posts.AnyAsync(x => x.Title == postTitle);
        }
    }
}
