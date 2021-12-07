using ASP.NET_MVC_Forum.Areas.Admin.Models.CommentReport;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASP.NET_MVC_Forum.Services.Business.CommentReport
{
    public interface ICommentReportBusinessService
    {
        public Task<List<CommentReportViewModel>> GenerateCommentReportViewModelListAsync(string reportStatus);

        public Task CensorCommentAsync(int commentId);

        public Task ReportCommentAsync(int commentId, string reasons);

        public Task AutoGenerateCommentReportAsync(string content, int commentId);

        public Task<bool> ReportExistsAsync(int reportId);

        public Task<bool> DeleteAsync(int reportId);

        public Task<bool> RestoreAsync(int reportId);

        public Task DeleteAndResolveAsync(int reportId);

        public Task HardCensorCommentAsync(int commentId);
    }
}
