namespace ASP.NET_MVC_Forum.Services.CommentReport
{
    using ASP.NET_MVC_Forum.Data.Models;
    using System.Linq;
    using System.Threading.Tasks;

    public interface ICommentReportService
    {
        public IQueryable<CommentReport> All(bool isDeleted = false);

        public Task<bool> DeleteAsync(int reportId);

        public Task<bool> ReportExistsAsync(int reportId);

        public Task<bool> RestoreAsync(int reportId);

        public Task AutoGenerateCommentReportAsync(string content, int commentId);

        public Task CensorCommentAsync(int postId);

        public Task HardCensorCommentAsync(int postId);

        public Task DeleteAndResolveAsync(int commentId, int reportId);
    }
}
