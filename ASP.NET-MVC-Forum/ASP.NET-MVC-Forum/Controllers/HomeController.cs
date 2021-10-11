namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Models;
    using ASP.NET_MVC_Forum.Models.Post;
    using ASP.NET_MVC_Forum.Services.Category;
    using ASP.NET_MVC_Forum.Services.Post;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    public class HomeController : Controller
    {
        private readonly ICategoryService categoryService;
        private readonly IPostService postsService;
        private readonly IMapper mapper;

        public HomeController(ICategoryService categories, IPostService postsService, IMapper mapper)
        {
            this.categoryService = categories;
            this.postsService = postsService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var vm = postsService
                .AllAsync(withUserIncluded: true)
                .GetAwaiter()
                .GetResult()
                .ProjectTo<PostPreviewViewModel>(mapper.ConfigurationProvider)
                .ToArray();

            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
