namespace ASP.NET_MVC_Forum.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using static ASP.NET_MVC_Forum.Data.DataConstants.RoleConstants;

    [Area("Admin")]
    [Authorize(Roles = AdminOrModerator)]
    public class HomeController : Controller
    {
        private readonly List<(string chartName,string url)> chartNameAndURL;
        public HomeController()
        {
            chartNameAndURL = new List<(string, string)>() 
            { 
                ( "Most commented posts", "/api/stats/most-commented-posts"),
                ( "Most liked posts", "/api/stats/most-liked-posts"),
                ( "Most reported posts", "/api/stats/most-reported-posts"),
                ( "Most posts by category", "/api/stats/most-posts-by-category")
            };
        }
        public IActionResult Index()
        {
            return View(chartNameAndURL);
        }
    }
}
