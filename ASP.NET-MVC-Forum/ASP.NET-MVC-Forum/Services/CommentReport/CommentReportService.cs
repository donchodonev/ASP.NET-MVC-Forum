namespace ASP.NET_MVC_Forum.Services.CommentReport
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Models;
    using ProfanityFilter.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class CommentReportService : ICommentReportService
    {
        private readonly ApplicationDbContext db;
        private readonly IProfanityFilter filter;

        public CommentReportService(ApplicationDbContext db, IProfanityFilter filter)
        {
            this.db = db;
            this.filter = filter;
        }
        public IQueryable<CommentReport> All(bool isDeleted = false)
        {
            return db.CommentReports.Where(x => x.IsDeleted == isDeleted);
        }

        public bool Delete(int reportId)
        {
            if (ReportExists(reportId))
            {
                var report = db.PostReports.First(x => x.Id == reportId);

                report.IsDeleted = true;
                report.ModifiedOn = DateTime.UtcNow;

                db.SaveChangesAsync().GetAwaiter();

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Restore(int reportId)
        {
            if (ReportExists(reportId))
            {
                var report = db.CommentReports.First(x => x.Id == reportId);

                report.IsDeleted = false;
                report.ModifiedOn = DateTime.UtcNow;

                db.SaveChangesAsync().GetAwaiter();

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ReportExists(int reportId)
        {
            return db.CommentReports.Any(x => x.Id == reportId);
        }

        public void AutoGenerateCommentReport(string content, int commentId)
        {
            if (filter.ContainsProfanity(content))
            {
                List<string> profaneWordsFound = GetProfanities(content);

                string reason = $"Profane words found in comment content: {string.Join(", ", profaneWordsFound)}";

                ReportComment(commentId, reason);
            }
        }

        public void ReportComment(int commentId, string reasons)
        {
            db.CommentReports.Add(new CommentReport() { CommentId = commentId, Reason = reasons });

            db.SaveChangesAsync().GetAwaiter();
        }

        public void CensorComment(int commentId)
        {
            var comment = db.Comments.First(x => x.Id == commentId);

            var profanities = GetProfanities(comment.Content);

            var censoredContent = filter.CensorString(comment.Content, '*');

            comment.Content = censoredContent;

            db.Update(comment);

            db.SaveChangesAsync().GetAwaiter();
        }

        public void HardCensorComment(int commentId)
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

            db.SaveChangesAsync().GetAwaiter();
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
