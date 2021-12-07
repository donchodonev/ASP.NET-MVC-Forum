namespace ASP.NET_MVC_Forum.Areas.Admin.Controllers
{
    using ASP.NET_MVC_Forum.Services.Business.Censor;
    using ASP.NET_MVC_Forum.Services.Business.PostReport;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Data.Constants.ClientMessage;
    using static ASP.NET_MVC_Forum.Data.Constants.RoleConstants;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ControllerExtensions;


    [Area("Admin")]
    [Authorize(Roles = AdminOrModerator)]
    public class PostReportsController : Controller
    {
        private readonly IPostReportBusinessService postReportService;
        private readonly ICensorService censorService;

        public PostReportsController(IPostReportBusinessService postReportService, ICensorService censorService)
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

                return this.RedirectToActionWithSuccessMessage(Success.ReportResolved, "PostReports", "Index");
            }

            return this.RedirectToActionWithErrorMessage(Error.ReportDoesNotExist, "PostReports", "Index");
        }

        public async Task<IActionResult> Restore(int reportId)
        {
            if (await postReportService.ReportExistsAsync(reportId))
            {
                await postReportService.RestoreAsync(reportId);

                return this.RedirectToActionWithSuccessMessage(Success.ReportRestored, "PostReports", "Index");
            }

            return this.RedirectToActionWithErrorMessage(Error.ReportDoesNotExist, "PostReports", "Index", new { reportStatus = "Active" });
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

            return this.RedirectToActionWithSuccessMessage(Success.PostCensored, "PostReports", "Index");
        }

        public async Task<IActionResult> DeleteAndResolve(int postId)
        {
            await postReportService.DeletePostAndResolveReportsAsync(postId);

            return this.RedirectToActionWithSuccessMessage(Success.PostCensoredAndResolved, "PostReports", "Index", new { reportStatus = "Deleted" });
        }
    }
}
