﻿namespace ASP.NET_MVC_Forum.Business.Contracts
{
    using ASP.NET_MVC_Forum.Business.Contracts.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Exceptions;

    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.ClientMessage.Error;

    public class PostReportValidationService : IPostReportValidationService
    {
        private readonly IPostReportRepository postReportRepo;

        public PostReportValidationService(IPostReportRepository postReportRepo)
        {
            this.postReportRepo = postReportRepo;
        }

        public async Task ValidateReportExistsAsync(int reportId)
        {
            if (!await postReportRepo.ExistsAsync(reportId))
            {
                throw new PostReportDoesNotExistException(REPORT_DOES_NOT_EXIST);
            }
        }

        public void ValidateReportNotNull(PostReport report)
        {
            if (report == null)
            {
                throw new PostReportDoesNotExistException(REPORT_DOES_NOT_EXIST);
            }
        }
    }
}
