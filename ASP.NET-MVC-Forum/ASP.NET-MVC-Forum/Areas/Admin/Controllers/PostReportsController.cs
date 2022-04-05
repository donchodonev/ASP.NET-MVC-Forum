﻿namespace ASP.NET_MVC_Forum.Web.Areas.Admin.Controllers
{
    using ASP.NET_MVC_Forum.Business.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.ClientMessage;
    using static ASP.NET_MVC_Forum.Domain.Constants.RoleConstants;
    using static ASP.NET_MVC_Forum.Web.Extensions.ControllerExtensions;


    [Area("Admin")]
    [Authorize(Roles = AdminOrModerator)]
    public class PostReportsController : Controller
    {
        private readonly IPostReportService postReportService;
        private readonly ICensorService censorService;

        public PostReportsController(IPostReportService postReportService, ICensorService censorService)
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

                return this.RedirectToActionWithSuccessMessage(Success.REPORT_RESOLVED, "PostReports", "Index");
            }

            return this.RedirectToActionWithErrorMessage(Error.REPORT_DOES_NOT_EXIST, "PostReports", "Index");
        }

        public async Task<IActionResult> Restore(int reportId)
        {
            if (await postReportService.ReportExistsAsync(reportId))
            {
                await postReportService.RestoreAsync(reportId);

                return this.RedirectToActionWithSuccessMessage(Success.REPORT_RESTORED, "PostReports", "Index");
            }

            return this.RedirectToActionWithErrorMessage(Error.REPORT_DOES_NOT_EXIST, "PostReports", "Index", new { reportStatus = "Active" });
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

            return this.RedirectToActionWithSuccessMessage(Success.POST_CENSORED, "PostReports", "Index");
        }

        public async Task<IActionResult> DeleteAndResolve(int postId)
        {
            await postReportService.DeletePostAndResolveReportsAsync(postId);

            return this.RedirectToActionWithSuccessMessage(Success.POST_CENSORED_AND_RESOLVED, "PostReports", "Index", new { reportStatus = "Deleted" });
        }
    }
}
