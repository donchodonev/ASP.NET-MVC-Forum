namespace ASP.NET_MVC_Forum.Areas.Admin.Controllers
{
    using ASP.NET_MVC_Forum.Areas.Admin.Models.PostReport;
    using ASP.NET_MVC_Forum.Services.CommentReport;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using static ASP.NET_MVC_Forum.Data.DataConstants.RoleConstants;


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
            List<PostReportViewModel> vm = new List<PostReportViewModel>();

            if (reportStatus == "Active")
            {
                vm = mapper.ProjectTo<PostReportViewModel>(commentReportService.All()).ToList();
            }
            else
            {
                vm = mapper.ProjectTo<PostReportViewModel>(commentReportService.All(isDeleted: true)).ToList();
            }

            return View(vm);
        }

        public IActionResult Delete(int reportId)
        {
            if (commentReportService.Delete(reportId))
            {
                TempData["Message"] = "Report has been marked as resolved !";
            }
            else
            {
                TempData["Message"] = "A report with such an ID does not exist";
            }

            return RedirectToAction("Index", "Reports");
        }

        public IActionResult Restore(int reportId)
        {
            if (commentReportService.Restore(reportId))
            {
                TempData["Message"] = "Report has been successfully restored !";
            }
            else
            {
                TempData["Message"] = "A report with such an ID does not exist";
            }

            return RedirectToAction("Index", "Reports", new { reportStatus = "Active" });
        }

        public IActionResult Censor(int commentId, bool withRegex)
        {
            if (withRegex)
            {
                commentReportService.HardCensorComment(commentId);
            }
            else
            {
                commentReportService.CensorComment(commentId);
            }

            TempData["Message"] = "The report has been successfully censored";

            return RedirectToAction("Index", "Reports");
        }
    }
}
