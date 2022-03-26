namespace ASP.NET_MVC_Forum.Data.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IPostReportDataService
    {
        public IQueryable<PostReport> All(bool isDeleted = false);

        public Task Update(PostReport report);

        public Task UpdateAll(ICollection<PostReport> report);

        public Task AddReport(PostReport report);

        public Task<PostReport> GetByIdAsync(int reportId, bool includePost = false);
    }
}
