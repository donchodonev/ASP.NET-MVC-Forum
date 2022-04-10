namespace ASP.NET_MVC_Forum.Business.Contracts.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;

    using System.Threading.Tasks;

    public interface IPostReportValidationService
    {
        public Task ValidateReportExistsAsync(int reportId);

        public void ValidateReportNotNull(PostReport report);
    }
}
