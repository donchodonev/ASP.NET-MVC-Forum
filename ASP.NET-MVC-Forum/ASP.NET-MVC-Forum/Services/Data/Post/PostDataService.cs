namespace ASP.NET_MVC_Forum.Services.Data.Post
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;
    public class PostDataService : IPostDataService
    {
        private readonly ApplicationDbContext db;

        public PostDataService(ApplicationDbContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Get all posts from database
        /// </summary>
        /// <param name="filters">A query filter enum used to materialize data from Post's navigational properties </param>
        /// <returns>Returns IQueryable<Post></returns>
        public IQueryable<Post> All(params PostQueryFilter[] filters)
        {
            var query = QueryBuilder(filters);

            return query;
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

            return post.Id;
        }

        /// <summary>
        /// Updates existing post
        /// </summary>
        /// <param name="post">The post to edit</param>
        /// <returns>void</returns>
        public async Task UpdatePostAsync(Post post)
        {
            db.Posts.Update(post);

            await db.SaveChangesAsync();
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

                return QueryBuilder(query, filters);
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

                return QueryBuilder(query, filters);
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

                return QueryBuilder(query, filters);
            });
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