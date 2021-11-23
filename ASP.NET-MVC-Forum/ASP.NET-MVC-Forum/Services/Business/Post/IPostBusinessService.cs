namespace ASP.NET_MVC_Forum.Services.Business.Post
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using ASP.NET_MVC_Forum.Data.Models;

    public interface IPostBusinessService
    {
        public Task<int> CreateNew(Post post, int userId);

        public Task Edit (Post post);

        public Task<bool> UserCanEdit(int userId, int postId, ClaimsPrincipal principal);

        public Task<bool> IsAuthor(int userId, int postId);
    }
}