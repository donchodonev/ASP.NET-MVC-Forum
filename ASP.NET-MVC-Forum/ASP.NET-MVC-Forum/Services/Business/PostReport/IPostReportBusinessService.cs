namespace ASP.NET_MVC_Forum.Web.Services.Business.PostReport
{
    using ASP.NET_MVC_Forum.Web.Areas.Admin.Models.PostReport;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPostReportBusinessService
    {
        public Task ReportAsync(int postId, string reason);

        public Task DeleteAsync(int id);

        public Task DeletePostAndResolveReportsAsync(int id);

        public Task RestoreAsync(int id);

        public Task AutoGeneratePostReportAsync(string title, string content, int postId);

        public Task<List<PostReportViewModel>> GeneratePostReportViewModelList(string reportStatus);

        public Task<bool> ReportExistsAsync(int reportId);
    }
}
