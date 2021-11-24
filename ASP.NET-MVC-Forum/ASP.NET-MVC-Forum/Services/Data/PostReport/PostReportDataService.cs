namespace ASP.NET_MVC_Forum.Services.Data.PostReport
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class PostReportDataService : IPostReportDataService
    {
        private readonly ApplicationDbContext db;

        public PostReportDataService(ApplicationDbContext db)
        {
            this.db = db;
        }
        public IQueryable<PostReport> All(bool isDeleted = false)
        {
            return db
                .PostReports
                .Where(x => x.IsDeleted == isDeleted);
        }

        public async Task Update(PostReport report)
        {
            db.Update(report);

            await db.SaveChangesAsync();
        }

        public async Task UpdateAll(ICollection<PostReport> reports)
        {
            db.UpdateRange(reports);

            await db.SaveChangesAsync();
        }

        public async Task<bool> ReportExists(int reportId)
        {
            return await db
                .PostReports
                .AnyAsync(x => x.Id == reportId);
        }

        public async Task AddReport(PostReport report)
        {
            db
            .PostReports
            .Add(report);

            await db.SaveChangesAsync();
        }

        public async Task<PostReport> GetByIdAsync(int reportId, bool includePost = false)
        {
            var report = db
                .PostReports
                .Where(x => x.Id == reportId);

            if (includePost)
            {
                report = report.Include(x => x.Post);
            }

            return await report.FirstOrDefaultAsync();
        }
    }
}
