namespace ASP.NET_MVC_Forum.Web.Services.Data.CommentReport
{
    using ASP.NET_MVC_Forum.Data.Models;
    using System.Linq;
    using System.Threading.Tasks;

    public interface ICommentReportDataService
    {
        public Task CreateAsync<T>(T report) where T : class;

        public IQueryable<CommentReport> All(bool isDeleted = false);

        public Task UpdateAsync<T>(T entity) where T : class;

        public Task<CommentReport> GetByIdAsync(int reportId, bool withCommentIncluded = false);

        public Task<Comment> GetCommentByIdAsync(int commentId);
    }
}
