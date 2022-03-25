namespace ASP.NET_MVC_Forum.Web.Services.Business.CommentReport
{
    using ASP.NET_MVC_Forum.Web.Areas.Admin.Models.CommentReport;
    using ASP.NET_MVC_Forum.Web.Services.Comment;
    using ASP.NET_MVC_Forum.Web.Services.Data.CommentReport;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using ProfanityFilter.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using System;
    using System.Text.RegularExpressions;

    public class CommentReportBusinessService : ICommentReportBusinessService
    {
        private readonly IMapper mapper;
        private readonly ICommentReportDataService reportData;
        private readonly IProfanityFilter filter;

        public CommentReportBusinessService(IMapper mapper, ICommentReportDataService reportData, IProfanityFilter filter)
        {
            this.mapper = mapper;
            this.reportData = reportData;
            this.filter = filter;
        }
        public async Task<List<CommentReportViewModel>> GenerateCommentReportViewModelListAsync(string reportStatus)
        {
            if (reportStatus == "Active")
            {
                return await mapper.ProjectTo<CommentReportViewModel>(reportData.All()).ToListAsync();
            }

            return await mapper.ProjectTo<CommentReportViewModel>(reportData.All(isDeleted: true)).ToListAsync();
        }

        public async Task CensorCommentAsync(int commentId)
        {
            var comment = await reportData.GetCommentByIdAsync(commentId);

            var censoredContent = filter.CensorString(comment.Content, '*');

            comment.Content = censoredContent;

            await reportData.UpdateAsync(comment);
        }

        public async Task ReportCommentAsync(int commentId, string reasons)
        {
            var comment = new CommentReport { CommentId = commentId, Reason = reasons };

            await reportData.CreateAsync(comment);
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

        public async Task<bool> ReportExistsAsync(int reportId)
        {
            return await reportData
                .All(isDeleted:true)
                .AnyAsync(x => x.Id == reportId);
        }

        public async Task<bool> DeleteAsync(int reportId)
        {
            if (await ReportExistsAsync(reportId))
            {
                var report = await reportData.GetByIdAsync(reportId);

                report.IsDeleted = true;
                report.ModifiedOn = DateTime.UtcNow;

                await reportData.UpdateAsync(report);

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> RestoreAsync(int reportId)
        {
            if (await ReportExistsAsync(reportId))
            {
                var report = await reportData.GetByIdAsync(reportId,withCommentIncluded:true);

                report.IsDeleted = false;
                report.ModifiedOn = DateTime.UtcNow;

                report.Comment.IsDeleted = false;
                report.Comment.ModifiedOn = DateTime.UtcNow;

                await reportData.UpdateAsync(report);

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task DeleteAndResolveAsync(int reportId)
        {
            var timeOfResolution = DateTime.UtcNow;

            var report = await reportData.GetByIdAsync(reportId,withCommentIncluded:true);

            report.IsDeleted = true;
            report.ModifiedOn = timeOfResolution;

            report.Comment.IsDeleted = true;
            report.Comment.ModifiedOn = timeOfResolution;

            await reportData.UpdateAsync(report);
        }

        public async Task HardCensorCommentAsync(int commentId)
        {
            var comment = await reportData.GetCommentByIdAsync(commentId);

            var profanities = GetProfanities(comment.Content);

            var censoredContent = comment.Content;

            foreach (var profanity in profanities)
            {
                censoredContent = Regex.Replace(censoredContent, $"\\w*{profanity}\\w*", "*****");
            }

            comment.Content = censoredContent;

            await reportData.UpdateAsync(comment);
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
