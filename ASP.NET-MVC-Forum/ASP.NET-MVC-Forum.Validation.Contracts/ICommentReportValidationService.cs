﻿namespace ASP.NET_MVC_Forum.Validation.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;

    using System.Threading.Tasks;

    public interface ICommentReportValidationService
    {
        public Task ValidateCommentReportExistsAsync(int reportId);

        public void ValidateCommentReportNotNull(CommentReport report);

        public void ValidateCommentReportStatus(string status);
    }
}
