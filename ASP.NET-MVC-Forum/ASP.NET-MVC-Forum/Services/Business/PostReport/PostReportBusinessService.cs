namespace ASP.NET_MVC_Forum.Services.Business.PostReport
{
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Services.Business.Censor;
    using ASP.NET_MVC_Forum.Services.Data.Post;
    using ASP.NET_MVC_Forum.Services.Data.PostReport;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class PostReportBusinessService : IPostReportBusinessService
    {
        private readonly IPostReportDataService data;
        private readonly IPostDataService postDataService;
        private readonly ICensorService censorService;

        public PostReportBusinessService(IPostReportDataService data, IPostDataService postDataService, ICensorService censorService)
        {
            this.data = data;
            this.postDataService = postDataService;
            this.censorService = censorService;
        }

        public async Task ReportAsync(int postId, string reason)
        {
            var report = new PostReport() { PostId = postId, Reason = reason };
            await data.AddReport(report);
        }

        public async Task DeleteAsync(int id)
        {
            var report = await data.GetByIdAsync(id);

            report.IsDeleted = true;
            report.ModifiedOn = DateTime.UtcNow;

            await data.Update(report);
        }

        public async Task RestoreAsync(int id)
        {
            var report = await data.GetByIdAsync(id, includePost: true);

            report.IsDeleted = false;
            report.ModifiedOn = DateTime.UtcNow;
            report.Post.IsDeleted = false;
            report.Post.ModifiedOn = DateTime.UtcNow;

            await data.Update(report);
        }

        public async Task AutoGeneratePostReportAsync(string title, string content, int postId)
        {
            if (censorService.ContainsProfanity(content))
            {
                List<string> profaneWordsFound = censorService.FindPostProfanities(title, content);

                string reason = $"Profane words found in post title and content: {string.Join(", ", profaneWordsFound)}";

                await ReportAsync(postId, reason);
            }
        }

        public async Task DeletePostAndResolveReportsAsync(int postId)
        {
            var postWithAllReports = await postDataService
                .GetByIdAsync(postId, PostQueryFilter.WithReports);

            await postDataService.DeleteAsync(postWithAllReports); // deletes just the post

            DeleteAllPostReports(postWithAllReports.Reports);

            await data.UpdateAll(postWithAllReports.Reports);
        }

        private ICollection<PostReport> DeleteAllPostReports(ICollection<PostReport> reports)
        {
            foreach (var report in reports)
            {
                report.IsDeleted = true;
                report.ModifiedOn = DateTime.UtcNow;
            }

            return reports;
        }
    }
}
