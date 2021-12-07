namespace ASP.NET_MVC_Forum.Areas.Admin.Controllers
{
    using ASP.NET_MVC_Forum.Areas.Admin.Models.CommentReport;
    using ASP.NET_MVC_Forum.Services.CommentReport;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Data.Constants.RoleConstants;


    [Area("Admin")]
    [Authorize(Roles = AdminOrModerator)]
    public class CommentReportsController : Controller
    {
        private readonly ICommentReportService commentReportService;
        private readonly IMapper mapper;

        public CommentReportsController(ICommentReportService commentReportService, IMapper mapper)
        {
            this.commentReportService = commentReportService;
            this.mapper = mapper;
        }

        public IActionResult Index(string reportStatus)
        {
            List<CommentReportViewModel> vm;

            if (reportStatus == "Active")
            {
                vm = mapper.ProjectTo<CommentReportViewModel>(commentReportService.All()).ToList();
            }
            else
            {
                vm = mapper.ProjectTo<CommentReportViewModel>(commentReportService.All(isDeleted: true)).ToList();
            }

            return View(vm);
        }

        public async Task<IActionResult> Resolve(int reportId)
        {
            if (await commentReportService.ReportExistsAsync(reportId))
            {
                await commentReportService.DeleteAsync(reportId);

                TempData["Message"] = "Report has been marked as resolved !";
            }
            else
            {
                TempData["Message"] = "A report with such an ID does not exist";
            }

            return RedirectToAction("Index", "CommentReports", new { reportStatus = "Deleted" });
        }

        public async Task<IActionResult> Restore(int reportId)
        {
            if (await commentReportService.RestoreAsync(reportId))
            {
                TempData["Message"] = "Report has been successfully restored !";
            }
            else
            {
                TempData["Message"] = "A report with such an ID does not exist";
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

            TempData["Message"] = "The comment has been successfully censored";

            return RedirectToAction("Index", "CommentReports", new { reportStatus = "Active" });
        }

        public async Task<IActionResult> DeleteAndResolve(int commentId, int reportId)
        {
            await commentReportService.DeleteAndResolveAsync(commentId, reportId);

            TempData["Message"] = "The comment has been successfully censored and report was marked as resolved";

            return RedirectToAction("Index", "CommentReports", new { reportStatus = "Deleted" });
        }
    }
}
