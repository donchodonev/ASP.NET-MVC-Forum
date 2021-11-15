namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Models;
    using ASP.NET_MVC_Forum.Models.Post;
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Services.Post;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    public class HomeController : Controller
    {
        private readonly IPostService postsService;
        private readonly IMapper mapper;

        public HomeController(IPostService postsService, IMapper mapper)
        {
            this.postsService = postsService;
            this.mapper = mapper;
        }

        public IActionResult Index()
        {
            var vm =
                 mapper.Map<List<PostPreviewViewModel>>(GetPosts())
                .ToList();

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<Post> GetPosts()
        {
            var posts = postsService.AllAsync(
                    PostQueryFilter.WithUser,
                    PostQueryFilter.WithIdentityUser,
                    PostQueryFilter.WithoutDeleted,
                    PostQueryFilter.AsNoTracking)
                    .GetAwaiter()
                    .GetResult()
                    .ToList();

            return posts;
        }
    }
}
