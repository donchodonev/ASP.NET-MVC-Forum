namespace ASP.NET_MVC_Forum.Business
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Business.Contracts.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.PostReport;

    using AutoMapper;

    using Microsoft.EntityFrameworkCore;

    using ProfanityFilter.Interfaces;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class PostReportService : IPostReportService
    {
        private readonly IPostReportRepository postReportRepo;
        private readonly IPostRepository postRepo;
        private readonly IMapper mapper;
        private readonly IPostValidationService postValidationService;
        private readonly IPostReportValidationService postReportValidationService;
        private readonly IProfanityFilter filter;

        public PostReportService(
            IPostReportRepository postReportRepo,
            IPostRepository postRepo,
            IMapper mapper,
            IPostValidationService postValidationService,
            IPostReportValidationService postReportValidationService,
            IProfanityFilter filter)
        {
            this.postReportRepo = postReportRepo;
            this.postRepo = postRepo;
            this.mapper = mapper;
            this.postValidationService = postValidationService;
            this.postReportValidationService = postReportValidationService;
            this.filter = filter;
        }

        public async Task ReportAsync(int postId, string reason)
        {
            await postValidationService.ValidatePostExistsAsync(postId);

            var report = new PostReport() { PostId = postId, Reason = reason };

            await postReportRepo.AddReport(report);
        }

        public async Task DeleteAsync(int id)
        {
            var report = await postReportRepo.GetByIdAsync(id);

            postReportValidationService.ValidateReportNotNull(report);

            report.IsDeleted = true;

            report.ModifiedOn = DateTime.UtcNow;

            await postReportRepo.UpdateAsync(report);
        }

        public async Task RestoreAsync(int id)
        {
            var report = await postReportRepo
                .GetById(id)
                .Include(x => x.Post)
                .FirstOrDefaultAsync();

            postReportValidationService.ValidateReportNotNull(report);

            report.IsDeleted = false;

            report.ModifiedOn = DateTime.UtcNow;

            report.Post.IsDeleted = false;

            report.Post.ModifiedOn = DateTime.UtcNow;

            await postReportRepo.UpdateAsync(report);
        }

        public async Task AutoGeneratePostReportAsync(string title, string content, int postId)
        {
            List<string> badWords = FindPostProfanities(title, content);

            if (badWords.Count > 0)
            {
                string reason = $"Profane words found in post title and content: {string.Join(", ", badWords)}";

                await ReportAsync(postId, reason);
            }
        }

        public async Task DeletePostAndResolveReportsAsync(int postId)
        {
            var postWithAllReports = await postRepo
                .GetById(postId)
                .Include(x => x.Reports)
                .FirstOrDefaultAsync();

            await postRepo.DeleteAsync(postWithAllReports);

            DeleteAllPostReports(postWithAllReports.Reports);

            await postReportRepo.UpdateAll(postWithAllReports.Reports);
        }

        public async Task<List<PostReportViewModel>> GeneratePostReportViewModelList(string reportStatus)
        {
            var postReports = postReportRepo
                .All()
                .Where(x => !x.IsDeleted);

            if (reportStatus == "Active")
            {
                return await mapper
                    .ProjectTo<PostReportViewModel>(postReports)
                    .ToListAsync();
            }

            var inactivePostReports = postReportRepo
                .All()
                .Where(x => x.IsDeleted);

            return await mapper
                .ProjectTo<PostReportViewModel>(inactivePostReports)
                .ToListAsync();
        }

        public Task<bool> ReportExistsAsync(int reportId)
        {
            return postReportRepo.ExistsAsync(reportId);
        }

        public List<string> FindPostProfanities(string title, string content)
        {
            string[] titleWords = title.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            string[] contentWords = content.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            List<string> badWords = new List<string>();

            badWords.AddRange(titleWords.Where(x => ContainsProfanity(x)));
            badWords.AddRange(contentWords.Where(x => ContainsProfanity(x)));

            return badWords;
        }

        public List<string> FindPostProfanities(string title, string content, string shortDescription)
        {
            string[] shortDescriptionWords = shortDescription.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            List<string> badWords = new List<string>();

            List<string> titleAndContentBadWords = FindPostProfanities(title, content);

            badWords.AddRange(titleAndContentBadWords);
            badWords.AddRange(shortDescriptionWords.Where(x => ContainsProfanity(x)));

            return badWords;
        }

        public bool ContainsProfanity(string term)
        {
            return filter.ContainsProfanity(term);
        }

        public Task CensorAsync(bool withRegex, int postId)
        {
            if (withRegex)
            {
                return HardCensorAsync(postId);
            }

            return SoftCensorAsync(postId);
        }

        private async Task SoftCensorAsync(int postId)
        {
            var post = await postRepo.GetByIdAsync(postId);

            var title = filter.CensorString(post.Title, '*');
            var htmlContent = filter.CensorString(post.HtmlContent, '*');
            var shortDescription = filter.CensorString(post.ShortDescription, '*');

            post.Title = title;
            post.HtmlContent = htmlContent;
            post.ShortDescription = shortDescription;

            await postRepo.UpdateAsync(post);
        }

        private async Task HardCensorAsync(int postId)
        {
            var post = await postRepo.GetByIdAsync(postId);

            var profanities = FindPostProfanities(post.Title, post.HtmlContent, post.ShortDescription);

            var title = post.Title;
            var htmlContent = post.HtmlContent;
            var shortDescription = post.ShortDescription;

            foreach (var profanity in profanities)
            {
                title = Regex.Replace(title, $"\\w*{profanity}\\w*", "*****");
                htmlContent = Regex.Replace(htmlContent, $"\\w*{profanity}\\w*", "*****");
                shortDescription = Regex.Replace(shortDescription, $"\\w*{profanity}\\w*", "*****");
            }

            post.Title = title;
            post.HtmlContent = htmlContent;
            post.ShortDescription = shortDescription;

            await postRepo.UpdateAsync(post);
        }

        private void DeleteAllPostReports(ICollection<PostReport> reports)
        {
            foreach (var report in reports)
            {
                report.IsDeleted = true;
                report.ModifiedOn = DateTime.UtcNow;
            }
        }
    }
}
