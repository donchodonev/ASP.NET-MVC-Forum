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
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Data.DataConstants.PostConstants;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ClaimsPrincipalExtensions;

    public class PostsController : Controller
    {
        private readonly IUserService userService;
        private readonly IPostService postService;
        private readonly ICategoryService categoryService;
        private readonly IMapper mapper;

        public PostsController(IUserService userService, IPostService postService, ICategoryService categoryService,IMapper mapper)
        {
            this.userService = userService;
            this.postService = postService;
            this.categoryService = categoryService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> ViewPost(int postId, string postTitle)
        {
            var post = await postService.GetByIdAsync(postId,withUserIncluded:true);

            var vm = mapper.Map<ViewPostViewModel>(post);

            return View(vm);
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
                TempData["ErrorMessage"] = $"The length of the post must be longer than {HtmlContentMinLength} symbols";
                return RedirectToAction("Add","Posts");
            }

            if (await postService.PostExistsAsync(data.Title))
            {
                TempData["ErrorMessage"] = $"A post with the title {data.Title} already exists";
                return RedirectToAction("Add", "Posts");
            }

            var baseUserId = await userService.GetBaseUserIdAsync(this.User.Id());

            var newPost = mapper.Map<Post>(data);

            var postId = await postService.AddPostAsync(newPost, baseUserId);

            return RedirectToAction("ViewPost",new {postId = postId, postTitle = data.Title });
        }
    }
}
