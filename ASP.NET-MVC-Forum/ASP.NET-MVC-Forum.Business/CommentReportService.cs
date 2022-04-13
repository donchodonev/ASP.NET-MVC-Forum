namespace ASP.NET_MVC_Forum.Business
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.CommentReport;
    using ASP.NET_MVC_Forum.Validation.Contracts;

    using AutoMapper;

    using Microsoft.EntityFrameworkCore;

    using ProfanityFilter.Interfaces;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class CommentReportService : ICommentReportService
    {
        private readonly IMapper mapper;
        private readonly ICommentReportRepository commentReportRepo;
        private readonly IProfanityFilter filter;
        private readonly ICommentRepository commentRepo;
        private readonly ICommentReportValidationService commentReportValidationService;

        public CommentReportService(
            IMapper mapper,
            ICommentReportRepository commentReportRepo,
            IProfanityFilter filter,
            ICommentRepository commentRepo,
            ICommentReportValidationService commentReportValidationService)
        {
            this.mapper = mapper;
            this.commentReportRepo = commentReportRepo;
            this.filter = filter;
            this.commentRepo = commentRepo;
            this.commentReportValidationService = commentReportValidationService;
        }
        public async Task<List<CommentReportViewModel>> GenerateCommentReportViewModelListAsync(string reportStatus)
        {
            var allCommentReports = commentReportRepo
                .All();

            var activeCommentReports = allCommentReports.Where(x => !x.IsDeleted);

            if (reportStatus == "Active")
            {
                return await mapper
                    .ProjectTo<CommentReportViewModel>(activeCommentReports)
                    .ToListAsync();
            }

            var inactiveCommentReports = commentReportRepo
                .All()
                .Where(x => x.IsDeleted);

            return await mapper
                .ProjectTo<CommentReportViewModel>(inactiveCommentReports)
                .ToListAsync();
        }


        public Task CensorCommentAsync(bool withRegex, int commentId)
        {
            if (withRegex)
            {
                return HardCensorCommentAsync(commentId);
            }
            else
            {
                return SoftCensorCommentAsync(commentId);
            }
        }

        public async Task ReportCommentAsync(int commentId, string reasons)
        {
            var commentReport = new CommentReport { CommentId = commentId, Reason = reasons };

            await commentReportRepo.AddAsync(commentReport);
        }

        public async Task AutoGenerateCommentReportAsync(string content, int commentId)
        {
            if (filter.ContainsProfanity(content))
            {
                List<string> profaneWordsFound = GetProfanities(content);

                string reason = string.Join(", ", profaneWordsFound);

                await ReportCommentAsync(commentId, reason);
            }
        }

        public Task<bool> ReportExistsAsync(int reportId)
        {
            return commentReportRepo.ExistsAsync(reportId);
        }

        public async Task DeleteAsync(int reportId)
        {
            var report = await commentReportRepo.GetByIdAsync(reportId);

            commentReportValidationService.ValidateCommentReportNotNull(report);

            report.IsDeleted = true;

            report.ModifiedOn = DateTime.UtcNow;

            await commentReportRepo.UpdateAsync(report);
        }

        public async Task RestoreAsync(int reportId)
        {
            var report = await commentReportRepo
                .All()
                .Where(x => x.Id == reportId)
                .Include(x => x.Comment)
                .FirstAsync();

            commentReportValidationService.ValidateCommentReportNotNull(report);

            report.IsDeleted = false;

            report.ModifiedOn = DateTime.UtcNow;

            report.Comment.IsDeleted = false;

            report.Comment.ModifiedOn = DateTime.UtcNow;

            await commentReportRepo.UpdateAsync(report);
        }

        public async Task DeleteAndResolveAsync(int reportId)
        {
            var timeOfResolution = DateTime.UtcNow;

            var report = await commentReportRepo
                .All()
                .Where(x => x.Id == reportId)
                .Include(x => x.Comment)
                .FirstAsync();

            report.IsDeleted = true;

            report.ModifiedOn = timeOfResolution;

            report.Comment.IsDeleted = true;

            report.Comment.ModifiedOn = timeOfResolution;

            await commentReportRepo.UpdateAsync(report);
        }

        private async Task HardCensorCommentAsync(int commentId)
        {
            var comment = await commentReportRepo.GetByCommentIdAsync(commentId);

            var profanities = GetProfanities(comment.Content);

            var censoredContent = comment.Content;

            foreach (var profanity in profanities)
            {
                censoredContent = Regex.Replace(censoredContent, $"\\w*{profanity}\\w*", "*****");
            }

            comment.Content = censoredContent;

            await commentRepo.UpdateAsync(comment);
        }
        private async Task SoftCensorCommentAsync(int commentId)
        {
            var comment = await commentReportRepo.GetByCommentIdAsync(commentId);

            var censoredContent = filter.CensorString(comment.Content, '*');

            comment.Content = censoredContent;

            await commentRepo.UpdateAsync(comment);
        }

        private List<string> GetProfanities(string content)
        {
            List<string> profaneWordsFound = filter
                .DetectAllProfanities(content)
                .ToList();

            return profaneWordsFound;
        }
    }
}
