namespace ASP.NET_MVC_Forum.Services.Business.PostReport
{
    using ASP.NET_MVC_Forum.Services.Business.Censor;
    using ASP.NET_MVC_Forum.Services.Data.PostReport;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class PostReportBusinessService : IPostReportBusinessService
    {
        private readonly IPostReportDataService data;
        private readonly ICensorService censorService;

        public PostReportBusinessService(IPostReportDataService data, ICensorService censorService)
        {
            this.data = data;
            this.censorService = censorService;
        }
        public async Task Delete(int id)
        {
            var report = await data.GetByIdAsync(id);

            report.IsDeleted = true;
            report.ModifiedOn = DateTime.UtcNow;

            await data.Update(report);
        }

        public async Task Restore(int id)
        {
            var report = await data.GetByIdAsync(id, includePost: true);

            report.IsDeleted = false;
            report.ModifiedOn = DateTime.UtcNow;
            report.Post.IsDeleted = false;
            report.Post.ModifiedOn = DateTime.UtcNow;

            await data.Update(report);
        }

        public async Task AutoGeneratePostReport(string title, string content, int postId)
        {
            if (censorService.ContainsProfanity(content))
            {
                List<string> profaneWordsFound = censorService.FindPostProfanities(title, content);

                string reason = $"Profane words found in post title and content: {string.Join(", ", profaneWordsFound)}";

                await data.ReportPost(postId, reason);
            }
        }
    }
}
