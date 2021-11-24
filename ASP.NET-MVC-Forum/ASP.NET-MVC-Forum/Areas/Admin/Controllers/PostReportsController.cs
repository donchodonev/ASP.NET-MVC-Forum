namespace ASP.NET_MVC_Forum.Areas.Admin.Controllers
{
    using ASP.NET_MVC_Forum.Areas.Admin.Models.PostReport;
    using ASP.NET_MVC_Forum.Services.Business.Censor;
    using ASP.NET_MVC_Forum.Services.Business.PostReport;
    using ASP.NET_MVC_Forum.Services.Data.PostReport;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static ASP.NET_MVC_Forum.Data.DataConstants.RoleConstants;


    [Area("Admin")]
    [Authorize(Roles = AdminOrModerator)]
    public class PostReportsController : Controller
    {
        private readonly IPostReportDataService postReportDataService;
        private readonly IPostReportBusinessService postReportBusinessService;
        private readonly ICensorService censorService;
        private readonly IMapper mapper;

        public PostReportsController(IPostReportDataService postReportDataService,
            IPostReportBusinessService postReportBusinessService,
            ICensorService censorService,
            IMapper mapper)
        {
            this.postReportDataService = postReportDataService;
            this.postReportBusinessService = postReportBusinessService;
            this.censorService = censorService;
            this.mapper = mapper;
        }

        public IActionResult Index(string reportStatus)
        {
            List<PostReportViewModel> vm;

            if (reportStatus == "Active")
            {
                vm = mapper.ProjectTo<PostReportViewModel>(postReportDataService.All()).ToList();
            }
            else
            {
                vm = mapper.ProjectTo<PostReportViewModel>(postReportDataService.All(isDeleted: true)).ToList();
            }

            return View(vm);
        }

        public async Task<IActionResult> Delete(int reportId)
        {
            if (await postReportDataService.ReportExists(reportId))
            {
                await postReportBusinessService.DeleteAsync(reportId);
                TempData["Message"] = "Report has been marked as resolved !";
            }
            else
            {
                TempData["Message"] = "A report with such an ID does not exist";
            }

            return RedirectToAction("Index", "PostReports");
        }

        public async Task<IActionResult> Restore(int reportId)
        {
            if (await postReportDataService.ReportExists(reportId))
            {
                await postReportBusinessService.RestoreAsync(reportId);
                TempData["Message"] = "Report has been successfully restored !";
            }
            else
            {
                TempData["Message"] = "A report with such an ID does not exist";
            }

            return RedirectToAction("Index", "PostReports", new { reportStatus = "Active" });
        }

        public async Task<IActionResult> Censor(int postId, bool withRegex)
        {
            if (withRegex)
            {
                await censorService.CensorPostWithRegexAsync(postId);
            }
            else
            {
                await censorService.CensorPostAsync(postId);
            }

            TempData["Message"] = "The post has been successfully censored";

            return RedirectToAction("Index", "PostReports");
        }

        public async Task<IActionResult> DeleteAndResolve(int postId)
        {
            await postReportBusinessService.DeletePostAndResolveReports(postId);

            TempData["Message"] = "The post has been successfully censored and all of it's reports were marked as resolved";

            return RedirectToAction("Index", "PostReports", new { reportStatus = "Deleted" });
        }
    }
}
