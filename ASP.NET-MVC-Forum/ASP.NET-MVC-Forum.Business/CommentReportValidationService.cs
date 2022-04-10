namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Business.Contracts.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Exceptions;

    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.ClientMessage.Error;

    public class CommentReportValidationService : ICommentReportValidationService
    {
        private readonly ICommentReportRepository commentReportRepo;

        public CommentReportValidationService(ICommentReportRepository commentReportRepo)
        {
            this.commentReportRepo = commentReportRepo;
        }

        public async Task ValidateCommentReportExistsAsync(int reportId)
        {
            if (!await commentReportRepo.ExistsAsync(reportId))
            {
                throw new CommentReportDoesNotExistException(REPORT_DOES_NOT_EXIST);
            }
        }

        public void ValidateCommentReportNotNull(CommentReport report)
        {
            if (report == null)
            {
                throw new CommentReportDoesNotExistException(REPORT_DOES_NOT_EXIST);
            }
        }
    }
}
