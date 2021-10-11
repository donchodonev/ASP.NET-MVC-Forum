namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Models.Category;
    using ASP.NET_MVC_Forum.Models.Post;
    using ASP.NET_MVC_Forum.Services.Category;
    using ASP.NET_MVC_Forum.Services.Post;
    using ASP.NET_MVC_Forum.Services.User;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Ganss.XSS;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ClaimsPrincipalExtensions;

    public class PostsController : Controller
    {
        private readonly IUserService userService;
        private readonly IPostService postService;
        private readonly ICategoryService categoryService;
        private readonly IMapper mapper;
        private readonly IHtmlSanitizer sanitizer;

        public PostsController(IUserService userService, IPostService postService, ICategoryService categoryService,IMapper mapper, IHtmlSanitizer sanitizer)
        {
            this.userService = userService;
            this.postService = postService;
            this.categoryService = categoryService;
            this.mapper = mapper;
            this.sanitizer = sanitizer;
        }

        public IActionResult All()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Add()
        {
            var addPostFormModel = new AddPostFormModel();

            var categories = await categoryService.AllAsync();

            var selectOptions = categories
                .ProjectTo<CategoryIdAndName>(mapper.ConfigurationProvider)
                .ToArray();
                
            addPostFormModel.Categories = selectOptions;

            return View(addPostFormModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromForm]AddPostFormModel data)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Add","Posts", data);
            }

            var baseUserId = await userService.GetBaseUserIdAsync(this.User.Id());

            var newPost = mapper.Map<Post>(data);

            newPost.UserId = baseUserId;

            var sanitizedHtml = sanitizer.Sanitize(newPost.HtmlContent);

            newPost.HtmlContent = sanitizedHtml;

            var postId = await postService.AddPostAsync(newPost);

            TempData["SuccessMessage"] = "Your post has been successfully created";

            return RedirectToAction("Index","Home");
        }
    }
}
