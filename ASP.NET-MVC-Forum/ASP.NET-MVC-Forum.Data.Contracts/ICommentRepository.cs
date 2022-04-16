namespace ASP.NET_MVC_Forum.Data.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Comment;

    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;

    public interface ICommentRepository
    {
        public Task<int> AddCommentAsync(CommentPostRequestModel commentData, string userId);

        public IQueryable<Comment> All();

        public IQueryable GetAllByPostId(int postId);

        public IQueryable<Comment> GetById(int id);

        public Task UpdateAsync(Comment entity);

        public Task<bool> ExistsAsync(int commentId);
    }
}
