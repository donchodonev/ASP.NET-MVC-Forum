namespace ASP.NET_MVC_Forum.Areas.Admin.Controllers
{
    using ASP.NET_MVC_Forum.Services.Business.Censor;
    using ASP.NET_MVC_Forum.Services.Business.PostReport;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Data.Constants.RoleConstants;


    [Area("Admin")]
    [Authorize(Roles = AdminOrModerator)]
    public class PostReportsController : Controller
    {
        private readonly IPostReportBusinessService postReportService;
        private readonly ICensorService censorService;

        public PostReportsController(IPostReportBusinessService postReportService,ICensorService censorService)
        {
            this.postReportService = postReportService;
            this.censorService = censorService;
        }

        public async Task<IActionResult> Index(string reportStatus)
        {
            var vm = await postReportService.GeneratePostReportViewModelList(reportStatus);
            return View(vm);
        }

        public async Task<IActionResult> Delete(int reportId)
        {
            if (await postReportService.ReportExistsAsync(reportId))
            {
                await postReportService.DeleteAsync(reportId);
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
            if (await postReportService.ReportExistsAsync(reportId))
            {
                await postReportService.RestoreAsync(reportId);
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
            await postReportService.DeletePostAndResolveReportsAsync(postId);

            TempData["Message"] = "The post has been successfully censored and all of it's reports were marked as resolved";

            return RedirectToAction("Index", "PostReports", new { reportStatus = "Deleted" });
        }
    }
}
