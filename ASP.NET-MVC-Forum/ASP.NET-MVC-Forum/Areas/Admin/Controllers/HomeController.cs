namespace ASP.NET_MVC_Forum.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using static ASP.NET_MVC_Forum.Data.DataConstants.RoleConstants;

    [Area("Admin")]
    [Authorize(Roles = AdminOrModerator)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
