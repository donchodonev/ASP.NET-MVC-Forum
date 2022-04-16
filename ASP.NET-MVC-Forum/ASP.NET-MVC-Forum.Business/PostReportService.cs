namespace ASP.NET_MVC_Forum.Business
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.PostReport;
    using ASP.NET_MVC_Forum.Validation.Contracts;

    using AutoMapper;

    using Microsoft.EntityFrameworkCore;

    using ProfanityFilter.Interfaces;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.CommonConstants;


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
            //validation done in ReportAsync method

            List<string> badWords = FindPostProfanities(title, content);

            if (badWords.Count > 0)
            {
                string reason = $"Profane words found in post title and content: {string.Join(", ", badWords)}";

                await ReportAsync(postId, reason);
            }
        }

        public async Task DeletePostAndResolveReportsAsync(int postId)
        {
            Post postWithAllReports = await postRepo
                .GetById(postId)
                .Include(x => x.Reports)
                .FirstOrDefaultAsync();

            postValidationService.ValidateNotNull(postWithAllReports);

            await postRepo.DeleteAsync(postWithAllReports);

            DeleteAllPostReports(postWithAllReports.Reports);

            await postReportRepo.UpdateAll(postWithAllReports.Reports);
        }

        public Task<List<PostReportViewModel>> GeneratePostReportViewModelListAsync(string reportStatus)
        {
            postReportValidationService.ValidateStatus(reportStatus);

            IQueryable<PostReport> query = reportStatus switch
            {
                ACTIVE_STATUS => postReportRepo
                                    .All()
                                    .Where(x => !x.IsDeleted),

                DELETED_STATUS => postReportRepo
                                    .All()
                                    .Where(x => x.IsDeleted)
            };

            return mapper
                .ProjectTo<PostReportViewModel>(query)
                .ToListAsync();
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

        public async Task CensorAsync(bool withRegex, int postId)
        {
            var post = await postRepo.GetByIdAsync(postId);

            postValidationService.ValidateNotNull(post);

            if (withRegex)
            {
                HardCensor(post);
            }
            else
            {
                SoftCensor(post);
            }

            await postRepo.UpdateAsync(post);
        }

        private bool ContainsProfanity(string term)
        {
            return filter.ContainsProfanity(term);
        }

        private void SoftCensor(Post post)
        {
            var title = filter.CensorString(post.Title, '*');
            var htmlContent = filter.CensorString(post.HtmlContent, '*');
            var shortDescription = filter.CensorString(post.ShortDescription, '*');

            post.Title = title;
            post.HtmlContent = htmlContent;
            post.ShortDescription = shortDescription;
        }

        private void HardCensor(Post post)
        {
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
