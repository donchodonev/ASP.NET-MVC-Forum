namespace ASP.NET_MVC_Forum.Services.Post
{
    using ASP.NET_MVC_Forum.Data.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IPostService
    {
        public Task<IQueryable<Post>> AllAsync(bool withCategoryIncluded = false, bool withUserIncluded = false, bool withIdentityUserIncluded = false);

        public Task<IQueryable<Post>> AllAsNoTrackingAsync(bool withCategoryIncluded = false, bool withUserIncluded = false, bool withIdentityUserIncluded = false);

        public Task<int> AddPostAsync(Post post, int baseUserId);

        public Task<Post> GetByIdAsync(int postId, bool withCategoryIncluded = false, bool withUserIncluded = false, bool withIdentityUserIncluded = false, bool withUserPostsIncluded = false);

        public Task<IQueryable<Post>> GetByCategoryAsync(int categoryId, bool withCategoryIncluded = false, bool withUserIncluded = false, bool withIdentityUserIncluded = false,
            bool withUserPostsIncluded = false);

        public Task<bool> PostExistsAsync(string postTitle);

        public Task<IQueryable<Post>> GetByUserIdAsync(int userId, bool withCategoryIncluded = false, bool withUserIncluded = false, bool withIdentityUserIncluded = false);

        public Task<IQueryable<Post>> GetByIdAsQueryableAsync(int postId, bool withCategoryIncluded = false, bool withUserIncluded = false, bool withIdentityUserIncluded = false);

        public Task<bool> UserCanEditAsync(int userId, int postId);

        public Task<Dictionary<string, bool>> GetPostChanges(Post originalPost, string newHtmlContent, string newTitle, int newCategoryId);

        public string SanitizeAndDecodeHtmlContent(string html);

        public Task EditPostAsync(Post post);
    }
}
