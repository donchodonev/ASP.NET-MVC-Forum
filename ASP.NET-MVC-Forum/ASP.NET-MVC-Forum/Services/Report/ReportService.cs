namespace ASP.NET_MVC_Forum.Services.Report
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Models;
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
            if (isDeleted)
            {
                return db.Reports.Where(x => x.IsDeleted);
            }

            return db.Reports;
        }
    }
}
