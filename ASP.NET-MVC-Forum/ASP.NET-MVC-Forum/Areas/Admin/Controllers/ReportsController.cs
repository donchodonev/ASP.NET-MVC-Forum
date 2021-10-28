namespace ASP.NET_MVC_Forum.Areas.Admin.Controllers
{
    using ASP.NET_MVC_Forum.Areas.Admin.Models.Report;
    using ASP.NET_MVC_Forum.Services.Report;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using static ASP.NET_MVC_Forum.Data.DataConstants.RoleConstants;


    [Area("Admin")]
    [Authorize(Roles = AdminOrModerator)]
    public class ReportsController : Controller
    {
        private readonly IReportService reportService;
        private readonly IMapper mapper;

        public ReportsController(IReportService reportService, IMapper mapper)
        {
            this.reportService = reportService;
            this.mapper = mapper;
        }

        public IActionResult Index(string reportStatus)
        {
            List<ReportViewModel> vm = new List<ReportViewModel>();

            if (reportStatus == "Active")
            {
                vm = mapper.ProjectTo<ReportViewModel>(reportService.All()).ToList();
            }
            else
            {
                vm = mapper.ProjectTo<ReportViewModel>(reportService.All(isDeleted:true)).ToList();
            }

            return View(vm);
        }
    }
}
