namespace ASP.NET_MVC_Forum.Services.Business.PostReport
{
    using ASP.NET_MVC_Forum.Services.Business.Censor;
    using ASP.NET_MVC_Forum.Services.Data.PostReport;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ASP.NET_MVC_Forum.Data.Models;

    public class PostReportBusinessService : IPostReportBusinessService
    {
        private readonly IPostReportDataService data;
        private readonly ICensorService censorService;

        public PostReportBusinessService(IPostReportDataService data, ICensorService censorService)
        {
            this.data = data;
            this.censorService = censorService;
        }

        public async Task ReportAsync(int postId, string reason)
        {
            var report = new PostReport() { PostId = postId, Reason = reason};
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
    }
}
