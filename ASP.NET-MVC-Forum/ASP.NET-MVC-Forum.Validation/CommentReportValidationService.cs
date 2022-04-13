namespace ASP.NET_MVC_Forum.Validation
{
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Exceptions;
    using ASP.NET_MVC_Forum.Validation.Contracts;

    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.ClientMessage.Error;
    using static ASP.NET_MVC_Forum.Domain.Constants.CommonConstants;

    public class CommentReportValidationService : ICommentReportValidationService
    {
        private readonly ICommentReportRepository commentReportRepo;

        public CommentReportValidationService(ICommentReportRepository commentReportRepo)
        {
            this.commentReportRepo = commentReportRepo;
        }

        public async Task ValidateExistsAsync(int reportId)
        {
            if (!await commentReportRepo.ExistsAsync(reportId))
            {
                throw new CommentReportDoesNotExistException(REPORT_DOES_NOT_EXIST);
            }
        }

        public void ValidateNotNull(CommentReport report)
        {
            if (report == null)
            {
                throw new CommentReportDoesNotExistException(REPORT_DOES_NOT_EXIST);
            }
        }

        public void ValidateStatus(string status)
        {
            if(status != ACTIVE_STATUS && status != DELETED_STATUS)
            {
                throw new InvalidReportStatusException(INVALID_REPORT_STATUS);
            }
        }
    }
}
