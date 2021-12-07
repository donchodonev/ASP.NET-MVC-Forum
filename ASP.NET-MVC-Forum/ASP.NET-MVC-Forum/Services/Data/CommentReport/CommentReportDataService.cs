namespace ASP.NET_MVC_Forum.Services.Data.CommentReport
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using ProfanityFilter.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class CommentReportDataService : ICommentReportDataService
    {
        private readonly ApplicationDbContext db;
        private readonly IProfanityFilter filter;

        public CommentReportDataService(ApplicationDbContext db, IProfanityFilter filter)
        {
            this.db = db;
            this.filter = filter;
        }
        public IQueryable<CommentReport> All(bool isDeleted = false)
        {
            return db.CommentReports.Where(x => x.IsDeleted == isDeleted);
        }

        public async Task<bool> DeleteAsync(int reportId)
        {
            if (await ReportExistsAsync(reportId))
            {
                var report = db.CommentReports.First(x => x.Id == reportId);

                report.IsDeleted = true;
                report.ModifiedOn = DateTime.UtcNow;

                db.SaveChangesAsync().Wait();

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
                var report = db.CommentReports.First(x => x.Id == reportId);

                report.IsDeleted = false;
                report.ModifiedOn = DateTime.UtcNow;

                var comment = db.Comments.First(x => x.Id == report.CommentId);
                comment.IsDeleted = false;
                comment.ModifiedOn = DateTime.UtcNow;

                db.Update(report);
                db.Update(comment);

                await db.SaveChangesAsync();

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ReportExistsAsync(int reportId)
        {
            return await db.CommentReports.AnyAsync(x => x.Id == reportId);
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

        public async Task ReportCommentAsync(int commentId, string reasons)
        {
            db.CommentReports.Add(new CommentReport() { CommentId = commentId, Reason = reasons });

            await db.SaveChangesAsync();
        }

        public async Task CensorCommentAsync(int commentId)
        {
            var comment = db.Comments.First(x => x.Id == commentId);

            var profanities = GetProfanities(comment.Content);

            var censoredContent = filter.CensorString(comment.Content, '*');

            comment.Content = censoredContent;

            db.Update(comment);

            await db.SaveChangesAsync();
        }

        public async Task DeleteAndResolveAsync(int commentId, int reportId)
        {
            var comment = db.Comments.First(x => x.Id == commentId);

            comment.IsDeleted = true;
            comment.ModifiedOn = DateTime.UtcNow;

            var report = db.CommentReports.First(x => x.Id == reportId);

            report.IsDeleted = true;
            report.ModifiedOn = DateTime.UtcNow;

            db.Update(comment);
            db.Update(report);

            await db.SaveChangesAsync();
        }

        public async Task HardCensorCommentAsync(int commentId)
        {
            var comment = db.Comments.First(x => x.Id == commentId);

            var profanities = GetProfanities(comment.Content);

            var censoredContent = comment.Content;

            foreach (var profanity in profanities)
            {
                censoredContent = Regex.Replace(censoredContent, $"\\w*{profanity}\\w*", "*****");
            }

            comment.Content = censoredContent;

            db.Update(comment);

            await db.SaveChangesAsync();
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
