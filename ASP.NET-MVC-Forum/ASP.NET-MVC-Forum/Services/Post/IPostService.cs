namespace ASP.NET_MVC_Forum.Services.Post
{
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Data.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public interface IPostService
    {
        public IQueryable<Post> SortAndOrder(IQueryable<Post> posts, int sortType, int sortOrder, string searchTerm, string category);

        public  IQueryable<Post> All(params PostQueryFilter[] filters);

        public Task<int> AddPostAsync(Post post, int baseUserId);

        public Task<Post> GetByIdAsync(int postId, params PostQueryFilter[] filters);

        public Task<IQueryable<Post>> GetByCategoryAsync(int categoryId, params PostQueryFilter[] filters);

        public Task<bool> PostExistsAsync(string postTitle);

        public Task<bool> PostExistsAsync(int postId);

        public Task<IQueryable<Post>> GetByUserIdAsync(int userId, params PostQueryFilter[] filters);

        public  Task<IQueryable<Post>> GetByIdAsQueryableAsync(int postId, params PostQueryFilter[] filters);

        public Task<bool> UserCanEditAsync(int userId, int postId, ClaimsPrincipal principal);

        public Dictionary<string, bool> GetPostChanges(Post originalPost, string newHtmlContent, string newTitle, int newCategoryId);

        public string SanitizeAndDecodeHtmlContent(string html);

        public Task EditPostAsync(Post post);

        public Task DeletePostAsync(int postId);

        public Task<bool?> IsPostDeleted(int postId, string postTitle);

        public Task AddPostReport(int postId, string reportReason);
    }
}
