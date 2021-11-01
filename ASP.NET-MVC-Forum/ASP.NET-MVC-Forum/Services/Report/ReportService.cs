namespace ASP.NET_MVC_Forum.Services.Report
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Models;
    using ProfanityFilter.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext db;
        private readonly IProfanityFilter filter;

        public ReportService(ApplicationDbContext db, IProfanityFilter filter)
        {
            this.db = db;
            this.filter = filter;
        }
        public IQueryable<Report> All(bool isDeleted = false)
        {
            return db.Reports.Where(x => x.IsDeleted == isDeleted);
        }

        public bool Delete(int reportId)
        {
            if (ReportExists(reportId))
            {
                var report = db.Reports.First(x => x.Id == reportId);

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
                var report = db.Reports.First(x => x.Id == reportId);

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
            return db.Reports.Any(x => x.Id == reportId);
        }

        public void AutoGeneratePostReport(string title, string content, int postId)
        {
            if (filter.ContainsProfanity(content))
            {
                List<string> profaneWordsFound = filter.DetectAllProfanities(content)
                    .ToList();

                profaneWordsFound.AddRange(filter.DetectAllProfanities(title));

                string reason = $"Profane words found in post title and content: {string.Join(", ", profaneWordsFound)}";

                ReportPost(postId, reason);
            }
        }

        public void ReportPost(int postId, string reasons)
        {
            db.Reports.Add(new Report() { PostId = postId, Reason = reasons });

            db.SaveChangesAsync().GetAwaiter();
        }
    }
}
