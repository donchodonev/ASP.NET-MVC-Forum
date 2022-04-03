namespace ASP.NET_MVC_Forum.Data.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;

    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IPostReportRepository
    {
        public IQueryable<PostReport> All();

        public Task UpdateAsync(PostReport report);

        public Task UpdateAll(ICollection<PostReport> reports);

        public Task AddReport(PostReport report);

        public Task<PostReport> GetByIdAsync(int reportId);

        public IQueryable<PostReport> GetById(int reportId);
    }
}
