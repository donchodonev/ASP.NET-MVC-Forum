namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Models.PostReport;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPostReportService
    {
        public Task ReportAsync(int postId, string reason);

        public Task DeleteAsync(int id);

        public Task RestoreAsync(int id);

        public Task AutoGeneratePostReportAsync(string title, string content, int postId);

        public Task DeletePostAndResolveReportsAsync(int postId);

        public Task<List<PostReportViewModel>> GeneratePostReportViewModelListAsync(string reportStatus);

        public List<string> FindPostProfanities(string title, string content);

        public List<string> FindPostProfanities(string title, string content, string shortDescription);

        public bool ContainsProfanity(string term);

        public Task CensorAsync(bool withRegex, int postId);
    }
}
