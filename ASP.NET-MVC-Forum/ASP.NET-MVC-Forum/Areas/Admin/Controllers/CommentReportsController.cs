namespace ASP.NET_MVC_Forum.Web.Areas.Admin.Controllers
{
    using ASP.NET_MVC_Forum.Web.Infrastructure.Extensions;
    using ASP.NET_MVC_Forum.Web.Services.Business.CommentReport;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Web.Data.Constants.ClientMessage;
    using static ASP.NET_MVC_Forum.Web.Data.Constants.RoleConstants;
    using static ASP.NET_MVC_Forum.Web.Infrastructure.Extensions.ControllerExtensions;

    [Area("Admin")]
    [Authorize(Roles = AdminOrModerator)]
    public class CommentReportsController : Controller
    {
        private readonly ICommentReportBusinessService commentReportService;

        public CommentReportsController(ICommentReportBusinessService commentReportService)
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

                return this.RedirectToActionWithSuccessMessage(Success.ReportResolved,"CommentReports","Index", new { reportStatus = "Deleted" });
            }

            return this.RedirectToActionWithErrorMessage(Error.ReportDoesNotExist, "CommentReports", "Index", new { reportStatus = "Deleted" });
        }

        public async Task<IActionResult> Restore(int reportId)
        {
            if (await commentReportService.RestoreAsync(reportId))
            {
                return this.RedirectToActionWithSuccessMessage(Success.ReportRestored, "CommentReports", "Index", new { reportStatus = "Active" });
            }

            return this.RedirectToActionWithSuccessMessage(Error.ReportDoesNotExist, "CommentReports", "Index", new { reportStatus = "Active" });
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

            return this.RedirectToActionWithSuccessMessage(Success.CommentReportCensored, "CommentReports", "Index", new { reportStatus = "Active" });
        }

        public async Task<IActionResult> DeleteAndResolve(int reportId)
        {
            await commentReportService.DeleteAndResolveAsync(reportId);

            return this.RedirectToActionWithSuccessMessage(Success.CommentReportCensoredAndResolved, "CommentReports", "Index", new { reportStatus = "Deleted" });
        }
    }
}
