namespace ASP.NET_MVC_Forum.Validation.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;

    using System.Threading.Tasks;

    public interface ICommentReportValidationService
    {
        public Task ValidateExistsAsync(int reportId);

        public void ValidateNotNull(CommentReport report);

        public void ValidateStatus(string status);
    }
}
