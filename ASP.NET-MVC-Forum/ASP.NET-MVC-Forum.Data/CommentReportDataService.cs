namespace ASP.NET_MVC_Forum.Data
{
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;

    using Microsoft.EntityFrameworkCore;

    using System.Linq;
    using System.Threading.Tasks;

    public class CommentReportDataService : ICommentReportDataService
    {
        private readonly ApplicationDbContext db;

        public CommentReportDataService(ApplicationDbContext db)
        {
            this.db = db;
        }
        public IQueryable<CommentReport> All(bool isDeleted = false)
        {
            return db.CommentReports.Where(x => x.IsDeleted == isDeleted);
        }

        public async Task<CommentReport> GetByIdAsync(int reportId, bool withCommentIncluded = false)
        {
            var query = db.CommentReports.Where(x => x.Id == reportId);

            if (withCommentIncluded)
            {
                return await query.Include(x => x.Comment).FirstOrDefaultAsync();
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task CreateAsync<T>(T report) where T : class
        {
            db.Add(report);
            await db.SaveChangesAsync();
        }


        public async Task UpdateAsync<T>(T entity) where T : class
        {
            db.Update(entity);

            await db.SaveChangesAsync();
        }

        public async Task<Comment> GetCommentByIdAsync(int commentId)
        {
            return await db.Comments.FirstOrDefaultAsync(x => x.Id == commentId);
        }
    }
}
