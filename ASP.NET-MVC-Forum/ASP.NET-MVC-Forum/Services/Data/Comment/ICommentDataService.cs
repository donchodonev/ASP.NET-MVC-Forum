namespace ASP.NET_MVC_Forum.Services.Data.Comment
{
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Services.Comment.Models;
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
