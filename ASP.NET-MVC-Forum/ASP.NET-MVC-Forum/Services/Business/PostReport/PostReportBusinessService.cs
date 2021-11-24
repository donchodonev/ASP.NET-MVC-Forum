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
            var report = await data.GetById(id);

            report.IsDeleted = true;
            report.ModifiedOn = DateTime.UtcNow;

            await data.Update(report);
        }
    }
}
