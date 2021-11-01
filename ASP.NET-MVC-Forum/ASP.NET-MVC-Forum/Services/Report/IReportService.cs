namespace ASP.NET_MVC_Forum.Services.Report
{
    using ASP.NET_MVC_Forum.Data.Models;
    using System.Linq;

    public interface IReportService
    {
        public IQueryable<Report> All(bool isDeleted = false);

        public bool Delete(int reportId);

        public bool ReportExists(int reportId);

        public bool Restore(int reportId);

        public void AutoGeneratePostReport(string title, string content, int postId);
    }
}
