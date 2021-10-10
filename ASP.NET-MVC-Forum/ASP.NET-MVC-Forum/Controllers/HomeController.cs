namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Models;
    using ASP.NET_MVC_Forum.Services.Category.Contracts;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public class HomeController : Controller
    {
        private readonly ICategoryService categories;

        public HomeController(ICategoryService categories)
        {
            this.categories = categories;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
