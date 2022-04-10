namespace ASP.NET_MVC_Forum.Business.Contracts.Contracts
{
    using System.Threading.Tasks;

    public interface ICommentReportValidationService
    {
        public Task ValidateCommentReportExistsAsync(int reportId);
    }
}
