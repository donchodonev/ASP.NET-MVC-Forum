namespace ASP.NET_MVC_Forum.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using ProfanityFilter;

    public class CommentsController : Controller
    {
        public IActionResult Index()
        {
            ProfanityFilter filter = new ProfanityFilter();
            return View();
        }
    }
}
