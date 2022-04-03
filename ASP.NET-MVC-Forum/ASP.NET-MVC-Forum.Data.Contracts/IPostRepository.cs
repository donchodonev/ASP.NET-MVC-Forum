namespace ASP.NET_MVC_Forum.Data.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;

    using System.Linq;
    using System.Threading.Tasks;

    public interface IPostRepository
    {
        public IQueryable<Post> All();

        public Task<int> AddPostAsync(Post post);

        public Task UpdateAsync(Post post);

        public IQueryable<Post> GetById(int postId);

        public IQueryable<Post> GetByCategoryId(int categoryId);

        public IQueryable<Post> GetByUserId(string userId);

        public Task<Post> GetByIdAsync(int postId);

        public Task<Post> GetByCategoryIdAsync(int categoryId);

        public Task<Post> GetByUserIdAsync(string userId);

        public Task<bool> ExistsAsync(string postTitle);

        public Task<bool> ExistsAsync(int postId);

        public Task DeleteAsync(Post post);
    }
}
