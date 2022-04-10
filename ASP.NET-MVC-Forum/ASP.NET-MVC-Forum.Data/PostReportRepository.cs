namespace ASP.NET_MVC_Forum.Data
{
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;

    using Microsoft.EntityFrameworkCore;

    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class PostReportRepository : IPostReportRepository
    {
        private readonly ApplicationDbContext db;

        public PostReportRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IQueryable<PostReport> All()
        {
            return db.PostReports;
        }

        public Task UpdateAsync(PostReport report)
        {
            db.Update(report);

            return db.SaveChangesAsync();
        }

        public Task UpdateAll(ICollection<PostReport> reports)
        {
            db.UpdateRange(reports);

            return db.SaveChangesAsync();
        }

        public Task AddReport(PostReport report)
        {
            db
            .PostReports
            .Add(report);

            return db.SaveChangesAsync();
        }

        public IQueryable<PostReport> GetById(int reportId)
        {
            return db
                .PostReports
                .Where(x => x.Id == reportId);
        }

        public Task<PostReport> GetByIdAsync(int reportId)
        {
            return db
                .PostReports
                .FirstOrDefaultAsync(x => x.Id == reportId);
        }

        public Task<bool> ExistsAsync(int reportId)
        {
            return db.PostReports.AnyAsync(x => x.Id == reportId);
        }
    }
}
