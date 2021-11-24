namespace ASP.NET_MVC_Forum.Services.Business.PostReport
{
    using System.Threading.Tasks;

    public interface IPostReportBusinessService
    {
        public Task ReportAsync(int postId, string reason);

        public Task DeleteAsync(int id);

        public Task DeletePostAndResolveReports(int id);

        public Task RestoreAsync(int id);

        public Task AutoGeneratePostReportAsync(string title, string content, int postId);
    }
}
