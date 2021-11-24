namespace ASP.NET_MVC_Forum.Services.Data.PostReport
{
    using ASP.NET_MVC_Forum.Data.Models;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IPostReportDataService
    {
        public IQueryable<PostReport> All(bool isDeleted = false);

        public Task Update(PostReport report);

        public Task<bool> ReportExists(int reportId);

        public void DeleteAndResolve(int postId);

        public Task ReportPost(int postId, string reasons);

        public Task<PostReport> GetByIdAsync(int reportId, bool includePost = false);
    }
}
