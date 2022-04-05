namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Models.PostReport;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPostReportService
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
