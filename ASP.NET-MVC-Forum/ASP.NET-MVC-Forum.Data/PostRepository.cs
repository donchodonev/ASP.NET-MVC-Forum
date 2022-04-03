namespace ASP.NET_MVC_Forum.Data
{
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;

    using Microsoft.EntityFrameworkCore;

    using System.Linq;
    using System.Threading.Tasks;

    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext db;

        public PostRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IQueryable<Post> All()
        {
            return db.Posts;
        }

        public async Task<int> AddPostAsync(Post post)
        {
            db.Posts.Add(post);

            await db.SaveChangesAsync();

            return post.Id;
        }

        public Task UpdateAsync(Post post)
        {
            db.Posts.Update(post);

            return db.SaveChangesAsync();
        }

        public IQueryable<Post> GetById(int postId)
        {
            return db
                .Posts
                .Where(x => x.Id == postId);
        }

        public IQueryable<Post> GetByCategoryId(int categoryId)
        {
            return db
                .Posts
                .Where(x => x.CategoryId == categoryId);
        }

        public IQueryable<Post> GetByUserId(int userId)
        {
            return db
                .Posts
                .Where(x => x.UserId == userId);
        }

        public Task<Post> GetByIdAsync(int postId)
        {
            return db
                .Posts
                .FirstOrDefaultAsync(x => x.Id == postId);
        }

        public Task<Post> GetByCategoryIdAsync(int categoryId)
        {
            return db
                .Posts
                .FirstOrDefaultAsync(x => x.CategoryId == categoryId);
        }

        public Task<Post> GetByUserIdAsync(int userId)
        {
            return db
                .Posts
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public Task<bool> ExistsAsync(string postTitle)
        {
            return db
                .Posts
                .AsNoTracking()
                .AnyAsync(x => x.Title == postTitle && !x.IsDeleted);
        }

        public Task<bool> ExistsAsync(int postId)
        {
            return db
                .Posts
                .AsNoTracking()
                .AnyAsync(x => x.Id == postId && !x.IsDeleted);
        }

        public Task DeleteAsync(Post post)
        {
            post.IsDeleted = true;

            db.Update(post);

            return db.SaveChangesAsync();
        }
    }
}
