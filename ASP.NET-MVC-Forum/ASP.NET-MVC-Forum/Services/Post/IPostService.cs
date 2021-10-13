namespace ASP.NET_MVC_Forum.Services.Post
{
    using ASP.NET_MVC_Forum.Data.Models;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IPostService
    {
        public Task<IQueryable<Post>> AllAsync(bool withCategoryIncluded = false, bool withUserIncluded = false, bool withIdentityUserIncluded = false); 

        public Task<int> AddPostAsync(Post post, int baseUserId);

        public Task<Post> GetById(int postId, bool withCategoryIncluded = false, bool withUserIncluded = false, bool withIdentityUserIncluded = false);
    }
}
