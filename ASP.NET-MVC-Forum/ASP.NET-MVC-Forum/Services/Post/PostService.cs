namespace ASP.NET_MVC_Forum.Services.Post
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Services.Enums;
    using ASP.NET_MVC_Forum.Services.PostReport;
    using Ganss.XSS;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web;

    public class PostService : IPostService
    {
        private readonly ApplicationDbContext db;
        private readonly IHtmlSanitizer sanitizer;
        private readonly IPostReportService reportService;

        public PostService(ApplicationDbContext db, IHtmlSanitizer sanitizer, IPostReportService reportService)
        {
            this.db = db;
            this.sanitizer = sanitizer;
            this.reportService = reportService;
        }

        /// <summary>
        /// Get all posts from database
        /// </summary>
        /// <param name="filters">A query filter enum used to materialize data from Post's navigational properties </param>
        /// <returns>Returns IQueryable<Post></returns>
        public async Task<IQueryable<Post>> AllAsync(params PostQueryFilter[] filters)
        {
            return
                await Task.Run(() =>
                {
                    var query = QueryBuilder(filters);

                    return query.OrderByDescending(x => x.CreatedOn);
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

            var sanitizedAndDecodedHtml = SanitizeAndDecodeHtmlContent(post.HtmlContent);

            var pattern = @"<.*?>";
            var replacement = string.Empty;

            var postDescriptionWithoutHtml = Regex.Replace(sanitizedAndDecodedHtml, pattern, replacement);

            string postShortDescription;

            if (postDescriptionWithoutHtml.Length < 300)
            {
                postShortDescription = postDescriptionWithoutHtml.Substring(0, postDescriptionWithoutHtml.Length) + "...";
            }
            else
            {
                postShortDescription = postDescriptionWithoutHtml.Substring(0, 300) + "...";
            }

            post.HtmlContent = sanitizedAndDecodedHtml;
            post.ShortDescription = postShortDescription;

            await db.Posts.AddAsync(post);

            await db.SaveChangesAsync();

            var savedPost = await db.Posts.FirstAsync(x => x == post);

            reportService.AutoGeneratePostReport(savedPost.Title, savedPost.HtmlContent, savedPost.Id);

            return savedPost.Id;
        }


        /// <summary>
        /// Edits post
        /// </summary>
        /// <param name="post">The post to edit</param>
        /// <returns>void</returns>
        public async Task EditPostAsync(Post post)
        {
            var sanitizedAndDecodedHtml = SanitizeAndDecodeHtmlContent(post.HtmlContent);

            var pattern = @"<.*?>";
            var replacement = string.Empty;

            var postDescriptionWithoutHtml = Regex.Replace(sanitizedAndDecodedHtml, pattern, replacement);

            string postShortDescription;

            if (postDescriptionWithoutHtml.Length < 300)
            {
                postShortDescription = postDescriptionWithoutHtml.Substring(0, postDescriptionWithoutHtml.Length) + "...";
            }
            else
            {
                postShortDescription = postDescriptionWithoutHtml.Substring(0, 300) + "...";
            }

            post.HtmlContent = sanitizedAndDecodedHtml;
            post.ShortDescription = postShortDescription;
            post.ModifiedOn = DateTime.UtcNow;

            db.Posts.Update(post);

            await db.SaveChangesAsync();

            reportService.AutoGeneratePostReport(post.Title, post.HtmlContent, post.Id);
        }


        public async Task<Post> GetByIdAsync(int postId, params PostQueryFilter[] filters)
        {
            var query = 
                 db
                .Posts
                .Where(x => x.Id == postId);

            query = QueryBuilder(query, filters);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IQueryable<Post>> GetByCategoryAsync(int categoryId, bool withCategoryIncluded = false, bool withUserIncluded = false, bool withIdentityUserIncluded = false,
            bool withUserPostsIncluded = false)
        {
            return await Task.Run(() =>
            {
                var query = db.Posts.Where(x => x.IsDeleted == false && x.CategoryId == categoryId);

                if (withUserIncluded)
                {
                    query = query.Include(x => x.User);
                }

                if (withUserPostsIncluded)
                {
                    query = query.Include(x => x.User.Posts);
                }

                if (withIdentityUserIncluded)
                {
                    query = query.Include(x => x.User.IdentityUser);
                }

                return query.OrderByDescending(x => x.CreatedOn);
            });
        }

        public async Task<bool> PostExistsAsync(string postTitle)
        {
            return await db.Posts.AnyAsync(x => x.Title == postTitle);
        }

        public async Task<bool> PostExistsAsync(int postId)
        {
            return await db.Posts.AnyAsync(x => x.Id == postId);
        }

        public async Task<IQueryable<Post>> GetByUserIdAsync(int userId, bool withCategoryIncluded = false, bool withUserIncluded = false, bool withIdentityUserIncluded = false)
        {
            return await Task.Run(() =>
            {
                var query = db.Posts.Where(x => x.IsDeleted == false && x.UserId == userId);

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

                return query.OrderByDescending(x => x.CreatedOn);
            });
        }

        public async Task<IQueryable<Post>> GetByIdAsQueryableAsync(int postId, bool withCategoryIncluded = false, bool withUserIncluded = false, bool withIdentityUserIncluded = false)
        {
            return await Task.Run(() =>
            {
                var query = db.Posts.Where(x => x.IsDeleted == false && x.Id == postId);

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

        public async Task<bool> UserCanEditAsync(int userId, int postId)
        {
            return await db.Posts.AnyAsync(x => x.Id == postId && x.UserId == userId);
        }

        public async Task<Dictionary<string, bool>> GetPostChanges(Post originalPost, string newHtmlContent, string newTitle, int newCategoryId)
        {
            return await Task.Run(() =>
            {
                var kvp = new Dictionary<string, bool>();

                if (originalPost.HtmlContent.Length != SanitizeAndDecodeHtmlContent(newHtmlContent).Length)
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
            });
        }

        public string SanitizeAndDecodeHtmlContent(string html)
        {
            return HttpUtility.HtmlDecode(sanitizer.Sanitize(html));
        }

        public async Task DeletePostAsync(int postId)
        {
            var currentTime = DateTime.UtcNow;

            var postToMarkAsDeleted = await db
                .Posts
                .Where(x => x.Id == postId)
                .Include(x => x.Reports)
                .FirstAsync();

            postToMarkAsDeleted.IsDeleted = true;

            foreach (var report in postToMarkAsDeleted.Reports)
            {
                report.IsDeleted = true;
                report.ModifiedOn = currentTime;
            }

            postToMarkAsDeleted.ModifiedOn = currentTime;


            db.Update(postToMarkAsDeleted);

            await db.SaveChangesAsync();
        }

        public async Task<bool?> IsPostDeleted(int postId, string postTitle)
        {
            var post = await db.Posts.FirstOrDefaultAsync(x => x.Id == postId && x.Title == postTitle);

            if (post == null)
            {
                return null;
            }

            return post.IsDeleted;
        }

        public async Task AddPostReport(int postId, string reportReason)
        {
            db.PostReports.Add(new PostReport() { PostId = postId, Reason = reportReason });

            await db.SaveChangesAsync();
        }

        private IQueryable<Post> QueryBuilder(params PostQueryFilter[] filters)
        {
            var query = db
            .Posts
            .AsQueryable();

            foreach (var filter in filters)
            {
                switch (filter)
                {
                    case PostQueryFilter.WithoutDeleted:
                        query = query.Where(x => x.IsDeleted == false);
                        break;
                    case PostQueryFilter.AsNoTracking:
                        query = query.AsNoTracking();
                        break;
                    case PostQueryFilter.WithCategory:
                        query = query.Include(x => x.Category);
                        break;
                    case PostQueryFilter.WithIdentityUser:
                        query = query
                            .Include(x => x.User)
                            .ThenInclude(user => user.IdentityUser);
                        break;
                    case PostQueryFilter.WithUser:
                        query = query.Include(x => x.User);
                        break;
                    case PostQueryFilter.WithComments:
                        query = query.Include(x => x.Comments);
                        break;
                    case PostQueryFilter.WithUserPosts:
                        query = query
                            .Include(x => x.User)
                            .ThenInclude(u => u.Posts);
                        break;
                    case PostQueryFilter.WithVotes:
                        query = query
                            .Include(x => x.Votes);
                        break;
                }
            }

            return query;
        }

        private IQueryable<Post> QueryBuilder(IQueryable<Post> posts,params PostQueryFilter[] filters)
        {
            foreach (var filter in filters)
            {
                switch (filter)
                {
                    case PostQueryFilter.WithoutDeleted:
                        posts = posts.Where(x => x.IsDeleted == false);
                        break;
                    case PostQueryFilter.AsNoTracking:
                        posts = posts.AsNoTracking();
                        break;
                    case PostQueryFilter.WithCategory:
                        posts = posts.Include(x => x.Category);
                        break;
                    case PostQueryFilter.WithIdentityUser:
                        posts = posts
                            .Include(x => x.User)
                            .ThenInclude(user => user.IdentityUser);
                        break;
                    case PostQueryFilter.WithUser:
                        posts = posts.Include(x => x.User);
                        break;
                    case PostQueryFilter.WithComments:
                        posts = posts.Include(x => x.Comments);
                        break;
                    case PostQueryFilter.WithUserPosts:
                        posts = posts
                            .Include(x => x.User)
                            .ThenInclude(u => u.Posts);
                        break;
                    case PostQueryFilter.WithVotes:
                        posts = posts
                            .Include(x => x.Votes);
                        break;
                }
            }

            return posts;
        }
    }
}