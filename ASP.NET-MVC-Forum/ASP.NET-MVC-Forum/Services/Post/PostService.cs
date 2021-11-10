﻿namespace ASP.NET_MVC_Forum.Services.Post
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


        /// <summary>
        /// Gets post from database via it's Id
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="filters"></param>
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
        /// <param name="categoryId"></param>
        /// <param name="filters"></param>
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
        /// <param name="postTitle"></param>
        /// <returns>Task<bool></returns>
        public async Task<bool> PostExistsAsync(string postTitle)
        {
            return await db
                .Posts
                .AsNoTracking()
                .AnyAsync(x => x.Title == postTitle);
        }

        /// <summary>
        /// Checks if post exists by post Id
        /// </summary>
        /// <param name="postId"></param>
        /// <returns>async Task<bool></returns>
        public async Task<bool> PostExistsAsync(int postId)
        {
            return await db
                .Posts
                .AsNoTracking()
                .AnyAsync(x => x.Id == postId);
        }

        /// <summary>
        /// Gets all user posts by user Id asynchronously
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="filters"></param>
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
        /// <param name="postId"></param>
        /// <param name="filters"></param>
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
        /// <param name="userId"></param>
        /// <param name="postId"></param>
        /// <returns>Task<bool></returns>
        public async Task<bool> UserCanEditAsync(int userId, int postId)
        {
            return await db
                .Posts
                .AsNoTracking()
                .AnyAsync(x => x.Id == postId && x.UserId == userId);
        }

        /// <summary>
        /// Checks which parts of a post have been changed during edit (if any)
        /// </summary>
        /// <param name="originalPost"></param>
        /// <param name="newHtmlContent"></param>
        /// <param name="newTitle"></param>
        /// <param name="newCategoryId"></param>
        /// <returns>Task<Dictionary<string, bool>></returns>
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

        /// <summary>
        /// Sanitizes then decodes given raw HTML string
        /// </summary>
        /// <param name="html"></param>
        /// <returns>string</returns>
        public string SanitizeAndDecodeHtmlContent(string html)
        {
            return HttpUtility.HtmlDecode(sanitizer.Sanitize(html));
        }

        /// <summary>
        /// Deletes post by post id
        /// </summary>
        /// <param name="postId"></param>
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
        /// <param name="postId"></param>
        /// <param name="postTitle"></param>
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

        /// <summary>
        /// Builds a query according to provided filters with a source query of type IQueryable<Post>
        /// </summary>
        /// <param name="posts">User pre-defined posts query to the database</param>
        /// <param name="filters">The array of filters of type PostQueryFilter</param>
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