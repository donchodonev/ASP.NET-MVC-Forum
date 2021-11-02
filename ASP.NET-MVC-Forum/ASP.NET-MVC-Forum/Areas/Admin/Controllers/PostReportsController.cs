namespace ASP.NET_MVC_Forum.Areas.Admin.Controllers
{
    using ASP.NET_MVC_Forum.Areas.Admin.Models.PostReport;
    using ASP.NET_MVC_Forum.Services.PostReport;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using static ASP.NET_MVC_Forum.Data.DataConstants.RoleConstants;


    [Area("Admin")]
    [Authorize(Roles = AdminOrModerator)]
    public class PostReportsController : Controller
    {
        private readonly IPostReportService postReportService;
        private readonly IMapper mapper;

        public PostReportsController(IPostReportService postReportService, IMapper mapper)
        {
            this.postReportService = postReportService;
            this.mapper = mapper;
        }

        public IActionResult Index(string reportStatus)
        {
            List<PostReportViewModel> vm = new List<PostReportViewModel>();

            if (reportStatus == "Active")
            {
                vm = mapper.ProjectTo<PostReportViewModel>(postReportService.All()).ToList();
            }
            else
            {
                vm = mapper.ProjectTo<PostReportViewModel>(postReportService.All(isDeleted: true)).ToList();
            }

            return View(vm);
        }

        public IActionResult Delete(int reportId)
        {
            if (postReportService.Delete(reportId))
            {
                TempData["Message"] = "Report has been marked as resolved !";
            }
            else
            {
                TempData["Message"] = "A report with such an ID does not exist";
            }

            return RedirectToAction("Index", "PostReports");
        }

        public IActionResult Restore(int reportId)
        {
            if (postReportService.Restore(reportId))
            {
                TempData["Message"] = "Report has been successfully restored !";
            }
            else
            {
                TempData["Message"] = "A report with such an ID does not exist";
            }

            return RedirectToAction("Index", "PostReports", new { reportStatus = "Active" });
        }

        public IActionResult Censor(int postId, bool withRegex)
        {
            if (withRegex)
            {
                postReportService.HardCensorPost(postId);
            }
            else
            {
                postReportService.CensorPost(postId);
            }

            TempData["Message"] = "The post has been successfully censored";

            return RedirectToAction("Index", "PostReports");
        }

        public IActionResult DeleteAndResolve(int postId)
        {
            postReportService.DeleteAndResolve(postId);

            TempData["Message"] = "The post has been successfully censored and report was marked as resolved";

            return RedirectToAction("Index", "PostReports", new { reportStatus = "Deleted" });
        }
    }
}
