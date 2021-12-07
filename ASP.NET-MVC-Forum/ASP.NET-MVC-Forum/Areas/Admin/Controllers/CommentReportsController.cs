namespace ASP.NET_MVC_Forum.Areas.Admin.Controllers
{
    using ASP.NET_MVC_Forum.Services.Business.CommentReport;
    using ASP.NET_MVC_Forum.Services.Data.CommentReport;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Data.Constants.RoleConstants;
    using static ASP.NET_MVC_Forum.Data.Constants.ClientMessage;


    [Area("Admin")]
    [Authorize(Roles = AdminOrModerator)]
    public class CommentReportsController : Controller
    {
        private readonly ICommentReportDataService commentReportService;
        private readonly ICommentReportBusinessService commetReportBusinessService;

        public CommentReportsController(ICommentReportDataService commentReportService, ICommentReportBusinessService commetReportBusinessService)
        {
            this.commentReportService = commentReportService;
            this.commetReportBusinessService = commetReportBusinessService;
        }

        public async Task<IActionResult> Index(string reportStatus)
        {
            var viewModel = await commetReportBusinessService.GenerateCommentReportViewModelListAsync(reportStatus);

            return View(viewModel);
        }

        public async Task<IActionResult> Resolve(int reportId)
        {
            if (await commentReportService.ReportExistsAsync(reportId))
            {
                await commentReportService.DeleteAsync(reportId);

                TempData["Message"] = Success.ReportResolved;
            }
            else
            {
                TempData["Message"] = Error.ReportDoesNotExist;
            }

            return RedirectToAction("Index", "CommentReports", new { reportStatus = "Deleted" });
        }

        public async Task<IActionResult> Restore(int reportId)
        {
            if (await commentReportService.RestoreAsync(reportId))
            {
                TempData["Message"] = Success.ReportRestored;
            }
            else
            {
                TempData["Message"] = Error.ReportDoesNotExist;;
            }

            return RedirectToAction("Index", "CommentReports", new { reportStatus = "Active" });
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

            TempData["Message"] = Success.ReportCensored;

            return RedirectToAction("Index", "CommentReports", new { reportStatus = "Active" });
        }

        public async Task<IActionResult> DeleteAndResolve(int commentId, int reportId)
        {
            await commentReportService.DeleteAndResolveAsync(commentId, reportId);

            TempData["Message"] = Success.ReportCensoredAndResolved;

            return RedirectToAction("Index", "CommentReports", new { reportStatus = "Deleted" });
        }
    }
}
