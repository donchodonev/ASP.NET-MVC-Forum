namespace ASP.NET_MVC_Forum.Services.Business.PostReport
{
    using ASP.NET_MVC_Forum.Services.Data.PostReport;
    using System;
    using System.Threading.Tasks;

    public class PostReportBusinessService : IPostReportBusinessService
    {
        private readonly IPostReportDataService data;

        public PostReportBusinessService(IPostReportDataService data)
        {
            this.data = data;
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
    }
}
