namespace ASP.NET_MVC_Forum.Services.PostReport
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using ProfanityFilter.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class PostReportService : IPostReportService
    {
        private readonly ApplicationDbContext db;
        private readonly IProfanityFilter filter;

        public PostReportService(ApplicationDbContext db, IProfanityFilter filter)
        {
            this.db = db;
            this.filter = filter;
        }
        public IQueryable<PostReport> All(bool isDeleted = false)
        {
            return db.PostReports.Where(x => x.IsDeleted == isDeleted);
        }

        public bool Delete(int reportId)
        {
            if (ReportExists(reportId))
            {
                var report = db.PostReports.First(x => x.Id == reportId);

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

        public bool Restore(int reportId)
        {
            if (ReportExists(reportId))
            {
                var report = db.PostReports.First(x => x.Id == reportId);

                report.IsDeleted = false;
                report.ModifiedOn = DateTime.UtcNow;

                var post = db.Posts.First(x => x.Id == report.PostId);
                post.IsDeleted = false;
                post.ModifiedOn = DateTime.UtcNow;

                db.Update(report);
                db.Update(post);

                db.SaveChangesAsync().Wait();

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ReportExists(int reportId)
        {
            return db.PostReports.Any(x => x.Id == reportId);
        }

        public void AutoGeneratePostReport(string title, string content, int postId)
        {
            if (filter.ContainsProfanity(content))
            {
                List<string> profaneWordsFound = GetProfanities(title, content);

                string reason = $"Profane words found in post title and content: {string.Join(", ", profaneWordsFound)}";

                ReportPost(postId, reason);
            }
        }

        public void ReportPost(int postId, string reasons)
        {
            db.PostReports.Add(new PostReport() { PostId = postId, Reason = reasons });

            db.SaveChangesAsync().Wait();
        }

        public void CensorPost(int postId)
        {
            var post = db.Posts.First(x => x.Id == postId);

            var profanities = GetProfanities(post.Title, post.HtmlContent);

            var censoredTitle = filter.CensorString(post.Title, '*');
            var censoredShortDescription = filter.CensorString(post.ShortDescription, '*');
            var censoredHtmlContent = filter.CensorString(post.HtmlContent, '*');

            post.Title = censoredTitle;
            post.HtmlContent = censoredHtmlContent;
            post.ShortDescription = censoredShortDescription;

            db.Update(post);

            db.SaveChangesAsync().GetAwaiter();
        }

        public void HardCensorPost(int postId)
        {
            var post = db.Posts.First(x => x.Id == postId);

            var profanities = GetProfanities(post.Title, post.HtmlContent,post.ShortDescription);

            var censoredTitle = post.Title;
            var censoredShortDescription = post.ShortDescription;
            var censoredHtmlContent = post.HtmlContent;

            foreach (var profanity in profanities)
            {
                censoredTitle = Regex.Replace(censoredTitle, $"\\w*{profanity}\\w*", "*****");
                censoredShortDescription = Regex.Replace(censoredShortDescription, $"\\w*{profanity}\\w*", "*****");
                censoredHtmlContent = Regex.Replace(censoredHtmlContent, $"\\w*{profanity}\\w*", "*****");
            }

            post.Title = censoredTitle;
            post.HtmlContent = censoredHtmlContent;
            post.ShortDescription = censoredShortDescription;

            db.Update(post);

            db.SaveChangesAsync().Wait();
        }

        public void DeleteAndResolve(int postId)
        {
            var post = db
                .Posts
                .Where(x => x.Id == postId)
                .Include(x => x.Reports)
                .First();

            post.IsDeleted = true;
            post.ModifiedOn = DateTime.UtcNow;

            foreach (var report in post.Reports)
            {
                report.IsDeleted = true;
                report.ModifiedOn = DateTime.UtcNow;
            }

            db.Update(post);

            db.SaveChangesAsync().Wait();
        }

        private List<string> GetProfanities(string title, string content)
        {
            List<string> profaneWordsFound = filter
                .DetectAllProfanities(content)
                .ToList();

            profaneWordsFound.AddRange(filter.DetectAllProfanities(title));

            return profaneWordsFound;
        }

        private List<string> GetProfanities(string title, string content, string shortDescription)
        {
            List<string> profaneWordsFound = filter
                .DetectAllProfanities(content.Substring(3,content.Length-3))
                .ToList();

            profaneWordsFound.AddRange(filter.DetectAllProfanities(title));
            profaneWordsFound.AddRange(filter.DetectAllProfanities(shortDescription));

            return profaneWordsFound;
        }
    }
}
