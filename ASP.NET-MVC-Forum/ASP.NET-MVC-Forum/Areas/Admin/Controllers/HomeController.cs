namespace ASP.NET_MVC_Forum.Web.Areas.Admin.Controllers
{
    using ASP.NET_MVC_Forum.Business.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using System.Collections.Generic;

    using static ASP.NET_MVC_Forum.Domain.Constants.RoleConstants;

    [Area("Admin")]
    [Authorize(Roles = ADMIN_OR_MODERATOR)]
    public class HomeController : Controller
    {
        private readonly List<(string chartName,string url)> chartNameAndURL;

        public HomeController(IChartService chartService)
        {
            chartNameAndURL = chartService.GenerateChartNamesAndUrls();
        }

        public IActionResult Index()
        {
            return View(chartNameAndURL);
        }
    }
}
