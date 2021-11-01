﻿namespace ASP.NET_MVC_Forum.Services.Post
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Services.Report;
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
        private readonly IReportService reportService;

        public PostService(ApplicationDbContext db, IHtmlSanitizer sanitizer, IReportService reportService)
        {
            this.db = db;
            this.sanitizer = sanitizer;
            this.reportService = reportService;
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
                var query = db.Posts
                .Where(x => x.IsDeleted == false);

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
        /// Returns a Post object with the given Id
        /// </summary>
        /// <param name="postId">Post id</param>
        /// <param name="withCategoryIncluded">True for Post's Category property to be included, false for null</param>
        /// <param name="withUserIncluded">True for Post's User property to be included, false for null</param>
        /// <param name="withIdentityUserIncluded">True for Post's User.IdentityUser property to be included, false for null</param>
        /// <returns></returns>
        public async Task<Post> GetByIdAsync(int postId, bool withCategoryIncluded = false, bool withUserIncluded = false, bool withIdentityUserIncluded = false,
            bool withUserPostsIncluded = false, bool withCommentsIncluded = false)
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

            if (withUserPostsIncluded)
            {
                query = query.Include(x => x.User.Posts);
            }

            if (withIdentityUserIncluded)
            {
                query = query.Include(x => x.User.IdentityUser);
            }
            if (withCommentsIncluded)
            {
                query = query.Include(x => x.Comments);
            }

            return await query.FirstOrDefaultAsync(x => x.Id == postId);
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

        public async Task<Dictionary<string,bool>> GetPostChanges(Post originalPost, string newHtmlContent, string newTitle, int newCategoryId)
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
            db.Reports.Add(new Report() { PostId = postId, Reason = reportReason});

            await db.SaveChangesAsync();
        }
    }
}