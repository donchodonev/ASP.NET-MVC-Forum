namespace ASP.NET_MVC_Forum.Data.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Comment;

    using System.Linq;
    using System.Threading.Tasks;

    public interface ICommentRepository
    {
        public Task<int> AddCommentAsync(RawCommentServiceModel commentData);

        public IQueryable<Comment> All();

        public IQueryable GetAllByPostId(int postId);

        public IQueryable<Comment> GetAllById(int id);

        public Task UpdateAsync(Comment entity);
    }
}
