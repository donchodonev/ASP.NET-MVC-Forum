namespace ASP.NET_MVC_Forum.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class PostsController : Controller
    {
        public IActionResult All()
        {
            return View();
        }

        [Authorize]
        public IActionResult Add()
        {
            return View();
        }
    }
}
