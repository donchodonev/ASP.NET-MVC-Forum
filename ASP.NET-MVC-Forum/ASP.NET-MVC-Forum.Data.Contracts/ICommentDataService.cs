namespace ASP.NET_MVC_Forum.Data.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Web.Services.Comment.Models;
    using System.Linq;
    using System.Threading.Tasks;

    public interface ICommentDataService
    {
        public IQueryable<Comment> All();

        public IQueryable GetAllByPostId(int postId, bool isDeleted = false, bool withIdentityUser = false, bool withBaseUser = false);

        public Task<int> AddComment(RawCommentServiceModel comment);

        public IQueryable<Comment> GetById(int id);

        public Task UpdateAsync<T>(T entity);
    }
}
