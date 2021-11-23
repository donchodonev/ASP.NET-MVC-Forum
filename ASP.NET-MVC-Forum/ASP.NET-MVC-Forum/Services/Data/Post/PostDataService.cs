namespace ASP.NET_MVC_Forum.Services.Data.Post
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Data.Models;
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
    public class PostDataService : IPostDataService
    {
        private readonly ApplicationDbContext db;
        private readonly IPostReportService reportService;
        private readonly IHtmlManipulator htmlManipulator;

        public PostDataService(ApplicationDbContext db, IPostReportService reportService, IHtmlManipulator htmlManipulator)
        {
            this.db = db;
            this.reportService = reportService;
            this.htmlManipulator = htmlManipulator;
        }

        /// <summary>
        /// Get all posts from database
        /// </summary>
        /// <param name="filters">A query filter enum used to materialize data from Post's navigational properties </param>
        /// <returns>Returns IQueryable<Post></returns>
        public IQueryable<Post> All(params PostQueryFilter[] filters)
        {
            var query = QueryBuilder(filters);

            return query.OrderByDescending(x => x.CreatedOn);
        }

        /// <summary>
        /// Adds a new post to the database
        /// </summary>
        /// <param name="post">Object of type Post</param>
        /// <param name="userId">Post's author user id</param>
        /// <returns>The newly added post's Id</returns>
        public async Task<int> AddPostAsync(Post post)
        {
            await db.Posts.AddAsync(post);

            await db.SaveChangesAsync();

            var savedPost = await db.Posts.FirstAsync(x => x == post);

            return post.Id;
        }


        /// <summary>
        /// Edits post
        /// </summary>
        /// <param name="post">The post to edit</param>
        /// <returns>void</returns>
        public async Task EditPostAsync(Post post)
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

            db.Posts.Update(post);

            await db.SaveChangesAsync();

            reportService.AutoGeneratePostReport(post.Title, post.HtmlContent, post.Id);
        }


        /// <summary>
        /// Gets post from database via it's Id
        /// </summary>
        /// <param name="postId">Post Id</param>
        /// <param name="filters">Array of comma-separated filters of type PostQueryFilter</param>
        /// <returns>Task<Post></returns>
        public async Task<Post> GetByIdAsync(int postId, params PostQueryFilter[] filters)
        {
            var query =
                 db
                .Posts
                .Where(x => x.Id == postId);

            query = QueryBuilder(query, filters);

            return await query.FirstOrDefaultAsync();
        }


        /// <summary>
        /// Get all posts from category matching the category id given to the method, filtered by chosen filters (if any)
        /// </summary>
        /// <param name="categoryId">Posts' category Id</param>
        /// <param name="filters">Array of comma-separated filters of type PostQueryFilter</param>
        /// <returns>Task<IQueryable<Post>></returns>
        public async Task<IQueryable<Post>> GetByCategoryAsync(int categoryId, params PostQueryFilter[] filters)
        {
            return await Task.Run(() =>
            {
                var query = db
                .Posts
                .Where(x => x.CategoryId == categoryId);

                query = QueryBuilder(query, filters);

                return query.OrderByDescending(x => x.CreatedOn);
            });
        }

        /// <summary>
        /// Checks if post exists by post title
        /// </summary>
        /// <param name="postTitle">Post's title</param>
        /// <returns>Task<bool></returns>
        public async Task<bool> PostExistsAsync(string postTitle)
        {
            return await db
                .Posts
                .AsNoTracking()
                .AnyAsync(x => x.Title == postTitle && !x.IsDeleted);
        }

        /// <summary>
        /// Checks if post exists by post Id
        /// </summary>
        /// <param name="postId">Post's Id</param>
        /// <returns>async Task<bool></returns>
        public async Task<bool> PostExistsAsync(int postId)
        {
            return await db
                .Posts
                .AsNoTracking()
                .AnyAsync(x => x.Id == postId && !x.IsDeleted);
        }

        /// <summary>
        /// Gets all user posts by user Id asynchronously
        /// </summary>
        /// <param name="userId">Post's author (user) Id</param>
        /// <param name="filters">Array of comma-separated filters of type PostQueryFilter</param>
        /// <returns>Task<IQueryable<Post>></returns>
        public async Task<IQueryable<Post>> GetByUserIdAsync(int userId, params PostQueryFilter[] filters)
        {
            return await Task.Run(() =>
            {
                var query = db.Posts.Where(x => x.UserId == userId);

                query = QueryBuilder(query, filters);

                return query.OrderByDescending(x => x.CreatedOn);
            });
        }

        /// <summary>
        /// Get post by post id filtered by chosen filters
        /// </summary>
        /// <param name="postId">Post's Id</param>
        /// <param name="filters">Array of comma-separated filters of type PostQueryFilter</param>
        /// <returns>Task<IQueryable<Post>></returns>
        public async Task<IQueryable<Post>> GetByIdAsQueryableAsync(int postId, params PostQueryFilter[] filters)
        {
            return await Task.Run(() =>
            {
                var query = db.Posts.Where(x => x.Id == postId);

                query = QueryBuilder(query, filters);

                return query;
            });
        }

        /// <summary>
        /// Checks if a post belongs to a cerain user by id
        /// </summary>
        /// <param name="userId">User's Id (author)</param>
        /// <param name="postId">Post's Id</param>
        /// <returns>Task<bool></returns>
        public async Task<bool> UserCanEditAsync(int userId, int postId, ClaimsPrincipal principal)
        {
            bool isAuthor = await db
                .Posts
                .AsNoTracking()
                .AnyAsync(x => x.Id == postId && x.UserId == userId);

            bool isAdminOrModerator = principal.IsAdminOrModerator();

            return isAuthor || isAdminOrModerator;
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

        /// <summary>
        /// Deletes post by post id
        /// </summary>
        /// <param name="postId">Post's Id</param>
        /// <returns>Task</returns>
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

        /// <summary>
        /// Checks if a post with this Id and Title is deleted
        /// </summary>
        /// <param name="postId">Post's Id</param>
        /// <param name="postTitle">Post's Title</param>
        /// <returns>Task<bool?></returns>
        public async Task<bool?> IsPostDeleted(int postId, string postTitle)
        {
            var post = await db
                .Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == postId && x.Title == postTitle);

            if (post == null)
            {
                return null;
            }

            return post.IsDeleted;
        }

        /// <summary>
        /// Adds a new post report
        /// </summary>
        /// <param name="postId">The post Id</param>
        /// <param name="reportReason">The reason for which the post is reported</param>
        /// <returns>Task</returns>
        public async Task AddPostReport(int postId, string reportReason)
        {
            db
            .PostReports
            .Add(new PostReport() { PostId = postId, Reason = reportReason });

            await db.SaveChangesAsync();
        }

        public IQueryable<Post> SortAndOrder(IQueryable<Post> posts, int sortType, int sortOrder, string searchTerm, string category)
        {
            //uncomment two variables underneath to view in debug possible order and sort options
            //var orderOptions = GetOrderType();
            //var sortOptions = GetSortOptions();

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

        /// <summary>
        /// Builds a query according to provided filters
        /// </summary>
        /// <param name="filters">The array of filters of type PostQueryFilter</param>
        /// <returns>IQueryable<Post></returns>
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
                        query = query.AsNoTracking().AsSplitQuery();
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
                    case PostQueryFilter.WithReports:
                        query = query
                            .Include(x => x.Reports);
                        break;
                }
            }

            return query;
        }

        /// <summary>
        /// Builds a query according to provided filters with a source query of type IQueryable<Post>
        /// </summary>
        /// <param name="posts">User pre-defined posts query to the database</param>
        /// <param name="filters">Array of comma-separated filters of type PostQueryFilter</param>
        /// <returns></returns>
        private IQueryable<Post> QueryBuilder(IQueryable<Post> posts, params PostQueryFilter[] filters)
        {
            if (posts.Count() == 0)
            {
                return posts;
            }

            foreach (var filter in filters)
            {
                switch (filter)
                {
                    case PostQueryFilter.WithoutDeleted:
                        posts = posts.Where(x => x.IsDeleted == false);
                        break;
                    case PostQueryFilter.AsNoTracking:
                        posts = posts.AsNoTracking().AsSplitQuery();
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
                    case PostQueryFilter.WithReports:
                        posts = posts
                            .Include(x => x.Reports);
                        break;
                }
            }

            return posts;
        }
    }
}