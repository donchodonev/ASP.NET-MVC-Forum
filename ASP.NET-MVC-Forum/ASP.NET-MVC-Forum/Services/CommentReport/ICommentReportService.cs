namespace ASP.NET_MVC_Forum.Services.CommentReport
{
    using ASP.NET_MVC_Forum.Data.Models;
    using System.Linq;

    public interface ICommentReportService
    {
        public IQueryable<CommentReport> All(bool isDeleted = false);

        public bool Delete(int reportId);

        public bool ReportExists(int reportId);

        public bool Restore(int reportId);

        public void AutoGenerateCommentReport(string content, int commentId);

        public void CensorComment(int postId);

        public void HardCensorComment(int postId);

        public void DeleteAndResolve(int commentId, int reportId);
    }
}
