﻿namespace ASP.NET_MVC_Forum.Services.Data.PostReport
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

    public class PostReportDataService : IPostReportDataService
    {
        private readonly ApplicationDbContext db;
        private readonly IProfanityFilter filter;

        public PostReportDataService(ApplicationDbContext db, IProfanityFilter filter)
        {
            this.db = db;
            this.filter = filter;
        }
        public IQueryable<PostReport> All(bool isDeleted = false)
        {
            return db
                .PostReports
                .Where(x => x.IsDeleted == isDeleted);
        }

        public async Task Update(PostReport report)
        {
            db.Update(report);

            await db.SaveChangesAsync();
        }

        public async Task<bool> ReportExists(int reportId)
        {
            return await db
                .PostReports
                .AnyAsync(x => x.Id == reportId);
        }

        public async Task AutoGeneratePostReport(string title, string content, int postId)
        {
            if (filter.ContainsProfanity(content))
            {
                List<string> profaneWordsFound = GetProfanities(title, content);

                string reason = $"Profane words found in post title and content: {string.Join(", ", profaneWordsFound)}";

                await ReportPost(postId, reason);
            }
        }

        public async Task ReportPost(int postId, string reasons)
        {
            db.PostReports.Add(new PostReport() { PostId = postId, Reason = reasons });

            await db.SaveChangesAsync();
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

            var profanities = GetProfanities(post.Title, post.HtmlContent, post.ShortDescription);

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
                .DetectAllProfanities(content.Substring(3, content.Length - 3))
                .ToList();

            profaneWordsFound.AddRange(filter.DetectAllProfanities(title));
            profaneWordsFound.AddRange(filter.DetectAllProfanities(shortDescription));

            return profaneWordsFound;
        }

        public async Task<PostReport> GetByIdAsync(int reportId, bool includePost = false)
        {
            var report = db
                .PostReports
                .Where(x => x.Id == reportId);

            if (includePost)
            {
                report = report.Include(x => x.Post);
            }

            return await report.FirstOrDefaultAsync();
        }
    }
}
