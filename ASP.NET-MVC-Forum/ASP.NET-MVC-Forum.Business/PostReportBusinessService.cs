namespace ASP.NET_MVC_Forum.Business
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Enums;
    using ASP.NET_MVC_Forum.Domain.Models.PostReport;
    using ASP.NET_MVC_Forum.Web.Services.Data.Post;
    using ASP.NET_MVC_Forum.Web.Services.Data.PostReport;

    using AutoMapper;

    using Microsoft.EntityFrameworkCore;

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class PostReportBusinessService : IPostReportBusinessService
    {
        private readonly IPostReportDataService data;
        private readonly IPostDataService postDataService;
        private readonly ICensorService censorService;
        private readonly IMapper mapper;

        public PostReportBusinessService(IPostReportDataService data, IPostDataService postDataService, ICensorService censorService, IMapper mapper)
        {
            this.data = data;
            this.postDataService = postDataService;
            this.censorService = censorService;
            this.mapper = mapper;
        }

        public async Task ReportAsync(int postId, string reason)
        {
            var report = new PostReport() { PostId = postId, Reason = reason };
            await data.AddReport(report);
        }

        public async Task DeleteAsync(int id)
        {
            var report = await data.GetByIdAsync(id);

            report.IsDeleted = true;
            report.ModifiedOn = DateTime.UtcNow;

            await data.Update(report);
        }

        public async Task RestoreAsync(int id)
        {
            var report = await data.GetByIdAsync(id, includePost: true);

            report.IsDeleted = false;
            report.ModifiedOn = DateTime.UtcNow;
            report.Post.IsDeleted = false;
            report.Post.ModifiedOn = DateTime.UtcNow;

            await data.Update(report);
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
            var postWithAllReports = await postDataService
                .GetByIdAsync(postId, PostQueryFilter.WithReports);

            await postDataService.DeleteAsync(postWithAllReports); // deletes just the post

            DeleteAllPostReports(postWithAllReports.Reports);

            await data.UpdateAll(postWithAllReports.Reports);
        }

        public async Task<List<PostReportViewModel>> GeneratePostReportViewModelList(string reportStatus)
        {
            if (reportStatus == "Active")
            {
                return await mapper.ProjectTo<PostReportViewModel>(data.All()).ToListAsync();
            }

            return await mapper.ProjectTo<PostReportViewModel>(data.All(isDeleted: true)).ToListAsync();
        }

        public async Task<bool> ReportExistsAsync(int reportId)
        {
            return await data.All()
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
