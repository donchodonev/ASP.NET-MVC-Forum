namespace ASP.NET_MVC_Forum.Web.Areas.Admin.Controllers
{
    using ASP.NET_MVC_Forum.Business.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.ClientMessage;
    using static ASP.NET_MVC_Forum.Domain.Constants.RoleConstants;
    using static ASP.NET_MVC_Forum.Web.Extensions.ControllerExtensions;


    [Area("Admin")]
    [Authorize(Roles = ADMIN_OR_MODERATOR)]
    public class PostReportsController : Controller
    {
        private readonly IPostReportService postReportService;

        public PostReportsController(IPostReportService postReportService)
        {
            this.postReportService = postReportService;
        }

        public async Task<IActionResult> Index(string reportStatus)
        {
            var vm = await postReportService.GeneratePostReportViewModelListAsync(reportStatus);

            return View(vm);
        }

        public async Task<IActionResult> Delete(int reportId)
        {
            await postReportService.DeleteAsync(reportId);

            return this.RedirectToActionWithSuccessMessage(
                Success.REPORT_RESOLVED,
                "PostReports",
                "Index", 
                new { reportStatus = "Active" });
        }

        public async Task<IActionResult> Restore(int reportId)
        {
            await postReportService.RestoreAsync(reportId);

            return this.RedirectToActionWithSuccessMessage(
                Success.REPORT_RESTORED,
                "PostReports",
                "Index",
                new { reportStatus = "Active" });
        }

        public async Task<IActionResult> Censor(int postId, bool withRegex)
        {
            await postReportService.CensorAsync(withRegex, postId);

            return this.RedirectToActionWithSuccessMessage(
                Success.POST_CENSORED,
                "PostReports",
                "Index",
                new { reportStatus = "Deleted" });
        }

        public async Task<IActionResult> DeleteAndResolve(int postId)
        {
            await postReportService.DeletePostAndResolveReportsAsync(postId);

            return this.RedirectToActionWithSuccessMessage(
                Success.POST_CENSORED_AND_RESOLVED,
                "PostReports",
                "Index",
                new { reportStatus = "Deleted" });
        }
    }
}
