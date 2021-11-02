namespace ASP.NET_MVC_Forum.Services.PostReport
{
    using ASP.NET_MVC_Forum.Data.Models;
    using System.Linq;

    public interface IPostReportService
    {
        public IQueryable<PostReport> All(bool isDeleted = false);

        public bool Delete(int reportId);

        public bool ReportExists(int reportId);

        public bool Restore(int reportId);

        public void AutoGeneratePostReport(string title, string content, int postId);

        public void CensorPost(int postId);

        public void HardCensorPost(int postId);

        public void DeleteAndResolve(int commentId, int reportId);
    }
}
