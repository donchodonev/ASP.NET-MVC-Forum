﻿namespace ASP.NET_MVC_Forum.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using static ASP.NET_MVC_Forum.Data.DataConstants.RoleConstants;

    [Area("Admin")]
    [Authorize(Roles = AdminOrModerator)]
    public class HomeController : Controller
    {
        private List<(string chartName,string url)> chartNameAndURL;
        public HomeController()
        {
            chartNameAndURL = new List<(string, string)>() 
            { 
                ( "Most commented posts", "/api/stats/most-commented-posts"),
                ( "Most liked posts", ""),
                ( "Most disled posts", ""),
                ( "Most reported posts", ""),
                ( "Most commented posts5", ""),
                ( "Most commented posts6", "")
            };
        }
        public IActionResult Index()
        {
            return View(chartNameAndURL);
        }
    }
}
