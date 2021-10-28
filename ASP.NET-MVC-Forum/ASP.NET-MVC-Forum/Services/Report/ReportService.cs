namespace ASP.NET_MVC_Forum.Services.Report
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Models;
    using System;
    using System.Linq;

    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext db;

        public ReportService(ApplicationDbContext db)
        {
            this.db = db;
        }
        public IQueryable<Report> All(bool isDeleted = false)
        {
            return db.Reports.Where(x => x.IsDeleted == isDeleted);
        }

        public bool Delete(int reportId)
        {
            if (ReportExists(reportId))
            {
                var report = db.Reports.First(x => x.Id == reportId);

                report.IsDeleted = true;
                report.ModifiedOn = DateTime.UtcNow;

                db.SaveChangesAsync().GetAwaiter();

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Restore(int reportId)
        {
            if (ReportExists(reportId))
            {
                var report = db.Reports.First(x => x.Id == reportId);

                report.IsDeleted = false;
                report.ModifiedOn = DateTime.UtcNow;

                db.SaveChangesAsync().GetAwaiter();

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ReportExists(int reportId)
        {
            return db.Reports.Any(x => x.Id == reportId);
        }
    }
}
