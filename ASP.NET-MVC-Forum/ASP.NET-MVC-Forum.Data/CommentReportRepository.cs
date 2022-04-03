namespace ASP.NET_MVC_Forum.Data
{
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;

    using Microsoft.EntityFrameworkCore;

    using System.Linq;
    using System.Threading.Tasks;

    public class CommentReportRepository : ICommentReportRepository
    {
        private readonly ApplicationDbContext db;

        public CommentReportRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IQueryable<CommentReport> All()
        {
            return db.CommentReports;
        }

        public Task<CommentReport> GetByIdAsync(int reportId)
        {
            var query = db.
                CommentReports
                .Where(x => x.Id == reportId);

            return query.FirstOrDefaultAsync();
        }

        public Task<Comment> GetByCommentIdAsync(int commentId)
        {
            return db
                .Comments
                .FirstOrDefaultAsync(x => x.Id == commentId);
        }

        public Task AddAsync(CommentReport report)
        {
            db.Add(report);

            return db.SaveChangesAsync();
        }

        public Task UpdateAsync(CommentReport report)
        {
            db.Update(report);

            return db.SaveChangesAsync();
        }
    }
}
