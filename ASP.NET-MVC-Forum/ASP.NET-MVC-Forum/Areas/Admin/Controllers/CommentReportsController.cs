namespace ASP.NET_MVC_Forum.Web.Areas.Admin.Controllers
{
    using ASP.NET_MVC_Forum.Web.Extensions;
    using ASP.NET_MVC_Forum.Business.Contracts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.ClientMessage;
    using static ASP.NET_MVC_Forum.Domain.Constants.RoleConstants;
    using static ASP.NET_MVC_Forum.Web.Extensions.ControllerExtensions;

    [Area("Admin")]
    [Authorize(Roles = AdminOrModerator)]
    public class CommentReportsController : Controller
    {
        private readonly ICommentReportService commentReportService;

        public CommentReportsController(ICommentReportService commentReportService)
        {
            this.commentReportService = commentReportService;
        }

        public async Task<IActionResult> Index(string reportStatus)
        {
            var viewModel = await commentReportService.GenerateCommentReportViewModelListAsync(reportStatus);

            return View(viewModel);
        }

        public async Task<IActionResult> Resolve(int reportId)
        {
            if (await commentReportService.ReportExistsAsync(reportId))
            {
                await commentReportService.DeleteAsync(reportId);

                return this.RedirectToActionWithSuccessMessage(Success.REPORT_RESOLVED,"CommentReports","Index", new { reportStatus = "Deleted" });
            }

            return this.RedirectToActionWithErrorMessage(Error.REPORT_DOES_NOT_EXIST, "CommentReports", "Index", new { reportStatus = "Deleted" });
        }

        public async Task<IActionResult> Restore(int reportId)
        {
            if (await commentReportService.RestoreAsync(reportId))
            {
                return this.RedirectToActionWithSuccessMessage(Success.REPORT_RESTORED, "CommentReports", "Index", new { reportStatus = "Active" });
            }

            return this.RedirectToActionWithSuccessMessage(Error.REPORT_DOES_NOT_EXIST, "CommentReports", "Index", new { reportStatus = "Active" });
        }

        public async Task<IActionResult> Censor(int commentId, bool withRegex)
        {
            if (withRegex)
            {
                await commentReportService.HardCensorCommentAsync(commentId);
            }
            else
            {
                await commentReportService.CensorCommentAsync(commentId);
            }

            return this.RedirectToActionWithSuccessMessage(Success.COMMENT_REPORT_CENSORED, "CommentReports", "Index", new { reportStatus = "Active" });
        }

        public async Task<IActionResult> DeleteAndResolve(int reportId)
        {
            await commentReportService.DeleteAndResolveAsync(reportId);

            return this.RedirectToActionWithSuccessMessage(Success.COMMENT_REPORT_CENSORED_AND_RESOLVED, "CommentReports", "Index", new { reportStatus = "Deleted" });
        }
    }
}
