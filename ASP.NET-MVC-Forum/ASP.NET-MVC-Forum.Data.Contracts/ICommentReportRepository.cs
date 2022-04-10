namespace ASP.NET_MVC_Forum.Data.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;

    using System.Linq;
    using System.Threading.Tasks;

    public interface ICommentReportRepository
    {
        public IQueryable<CommentReport> All();

        public Task<bool> ExistsAsync(int reportId);

        public Task<CommentReport> GetByIdAsync(int reportId);

        public Task<Comment> GetByCommentIdAsync(int commentId);

        public Task AddAsync(CommentReport report);

        public Task UpdateAsync(CommentReport report);
    }
}
