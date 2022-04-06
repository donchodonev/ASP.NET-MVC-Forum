namespace ASP.NET_MVC_Forum.Business
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Business.Contracts.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Enums;
    using ASP.NET_MVC_Forum.Domain.Models.PostReport;

    using AutoMapper;

    using Microsoft.EntityFrameworkCore;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class PostReportService : IPostReportService
    {
        private readonly IPostReportRepository postReportRepo;
        private readonly IPostRepository postRepo;
        private readonly ICensorService censorService;
        private readonly IMapper mapper;
        private readonly IPostValidationService postValidationService;

        public PostReportService(
            IPostReportRepository postReportRepo, 
            IPostRepository postRepo,
            ICensorService censorService, 
            IMapper mapper,
            IPostValidationService postValidationService)
        {
            this.postReportRepo = postReportRepo;
            this.postRepo = postRepo;
            this.censorService = censorService;
            this.mapper = mapper;
            this.postValidationService = postValidationService;
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

            report.IsDeleted = false;

            report.ModifiedOn = DateTime.UtcNow;

            report.Post.IsDeleted = false;

            report.Post.ModifiedOn = DateTime.UtcNow;

            await postReportRepo.UpdateAsync(report);
        }

        public async Task AutoGeneratePostReportAsync(string title, string content, int postId)
        {
            if (censorService.ContainsProfanity(content))
            {
                List<string> profaneWordsFound = censorService.FindPostProfanities(title, content);

                string reason = $"Profane words found in post title and content: {string.Join(", ", profaneWordsFound)}";

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

        public async Task<bool> ReportExistsAsync(int reportId)
        {
            return await postReportRepo
                .All()
                .AnyAsync(x => x.Id == reportId && !x.IsDeleted);
        }

        private ICollection<PostReport> DeleteAllPostReports(ICollection<PostReport> reports)
        {
            foreach (var report in reports)
            {
                report.IsDeleted = true;
                report.ModifiedOn = DateTime.UtcNow;
            }

            return reports;
        }
    }
}
