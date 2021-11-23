namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Data.Enums;
    using ASP.NET_MVC_Forum.Data.Models;
    using ASP.NET_MVC_Forum.Models.Post;
    using ASP.NET_MVC_Forum.Services.Business.Post;
    using ASP.NET_MVC_Forum.Services.Category;
    using ASP.NET_MVC_Forum.Services.Data.Post;
    using ASP.NET_MVC_Forum.Services.PostReport;
    using ASP.NET_MVC_Forum.Services.User;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using static ASP.NET_MVC_Forum.Data.DataConstants.PostConstants;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ClaimsPrincipalExtensions;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ControllerExtensions;

    public class PostsController : Controller
    {
        private const string PostDeletedMessege = "Your post has already been successfully deleted. Please allow 60 seconds to pass after which will stop being displayed";
        private const string ReportThankYouMessage = "Thank you for your report, our moderators will review it as quickly as possible !";

        private readonly IUserService userService;
        private readonly IPostDataService postDataService;
        private readonly ICategoryService categoryService;
        private readonly IMapper mapper;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IPostBusinessService postBusinessService;
        private readonly IPostReportService postReportService;

        public PostsController(
            IUserService userService,
            IPostDataService postDataService,
            ICategoryService categoryService,
            IMapper mapper,
            SignInManager<IdentityUser> signInManager,
            IPostBusinessService postBusinessService,
            IPostReportService postReportService)
        {
            this.userService = userService;
            this.postDataService = postDataService;
            this.categoryService = categoryService;
            this.mapper = mapper;
            this.signInManager = signInManager;
            this.postBusinessService = postBusinessService;
            this.postReportService = postReportService;
        }

        public async Task<IActionResult> ViewPost(int postId)
        {
            var post = await postDataService.GetByIdAsync(postId,
                PostQueryFilter.WithIdentityUser,
                PostQueryFilter.WithUserPosts,
                PostQueryFilter.WithComments,
                PostQueryFilter.WithVotes);

            if (post == null)
            {
                return BadRequest();
            }

            var vm = mapper.Map<ViewPostViewModel>(post);

            if (signInManager.IsSignedIn(this.User))
            {
                vm.UserLastVoteChoice = await GetUserLastVote(post, vm, this.User);
            }

            return View(vm);
        }

        [Authorize]
        public IActionResult Add()
        {
            var vm = PrepareAddFormDataOnGetAsync();

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromForm] AddPostFormModel data)
        {
            if (!ModelState.IsValid)
            {
                TempData["Title"] = data.Title;
                TempData["ErrorMessage"] = $"The length of the post must be longer than {HtmlContentMinLength} symbols";
                return RedirectToAction("Add", "Posts");
            }

            if (await postDataService.PostExistsAsync(data.Title))
            {
                TempData["ErrorMessage"] = $"A post with the title \"{data.Title}\" already exists";
                TempData["HtmlContent"] = data.HtmlContent;
                return RedirectToAction("Add", "Posts");
            }

            var newPost = await AddPostAsync(data);

            await postReportService.AutoGeneratePostReport(newPost.Title, newPost.HtmlContent, newPost.Id);

            return RedirectToAction("ViewPost", new { postId = newPost.Id, postTitle = newPost.Title });
        }

        [Authorize]
        public async Task<IActionResult> Edit(int postId)
        {
            await ValidatePostOwnership(postId, this.User);

            var post = postDataService
                .GetByIdAsQueryable(postId, PostQueryFilter.WithCategory);

            var vm = mapper
                .ProjectTo<EditPostFormModel>(post)
                .First();

            vm.Categories = GetCategoryIdAndNameCombinations();

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] EditPostFormModel data)
        {
            await ValidatePostOwnership(data.PostId, this.User);

            var originalPost = await postDataService.GetByIdAsync(data.PostId);

            var postChanges = postBusinessService.GetPostChanges(originalPost, data.HtmlContent, data.Title, data.CategoryId);

            if (postChanges.Count == 0)
            {
                return RedirectToAction("Edit", "Posts", new { postId = data.PostId });
            }

            AddPostChanges(originalPost, data, postChanges);

            await postBusinessService.Edit(originalPost);

            return RedirectToAction("ViewPost", new { postId = data.PostId, postTitle = data.Title });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int postId, string postTitle)
        {
            await ValidatePostOwnership(postId, this.User);

            var isPostDeleted = await postDataService.IsPostDeleted(postId, postTitle);

            if (isPostDeleted.Value == true)
            {
                this.RedirectToActionWithErrorMessage(PostDeletedMessege, "Index", "Home");
            }
            else if (!isPostDeleted.HasValue)
            {
                BadRequest();
            }

            await postBusinessService.Delete(postId);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Report([FromForm] string content, int postId)
        {
            if (!await postDataService.PostExistsAsync(postId))
            {
                return BadRequest();
            }

            await postReportService.ReportPost(postId, content);

            return this.RedirectToActionWithMessage(ReportThankYouMessage, "Home", "Index");
        }

        private AddPostFormModel PrepareAddFormDataOnGetAsync()
        {
            var addPostFormModel = new AddPostFormModel();

            var selectOptions = GetCategoryIdAndNameCombinations();

            addPostFormModel.Categories = selectOptions;

            if (TempData.ContainsKey("HtmlContent"))
            {
                addPostFormModel.HtmlContent = TempData["HtmlContent"].ToString();
            }
            if (TempData.ContainsKey("Title"))
            {
                addPostFormModel.Title = TempData["Title"].ToString();
            }

            return addPostFormModel;
        }

        private CategoryIdAndNameViewModel[] GetCategoryIdAndNameCombinations()
        {
            var categories = categoryService.All();

            var selectOptions = categories
                .ProjectTo<CategoryIdAndNameViewModel>(mapper.ConfigurationProvider)
                .ToArray();

            return selectOptions;
        }

        private async Task<Post> AddPostAsync(AddPostFormModel data)
        {
            var baseUserId = await userService.GetBaseUserIdAsync(this.User.Id());

            var newPost = mapper.Map<Post>(data);

            await postBusinessService.CreateNew(newPost, baseUserId);

            return newPost;
        }

        private async Task ValidatePostOwnership(int postId, ClaimsPrincipal principal)
        {
            var userId = await userService.GetBaseUserIdAsync(User.Id());

            if (!await postBusinessService.UserCanEdit(userId, postId, principal))
            {
                RedirectToAction("Index", "Home");
            }
        }

        private async Task<int> GetUserLastVote(Post post, ViewPostViewModel vm, ClaimsPrincipal user)
        {
            int userId = await userService
                .GetBaseUserIdAsync(user.Id());


            var userLastVote = post.Votes.FirstOrDefault(x => x.UserId == userId);

            if (userLastVote != null)
            {
                return (int)userLastVote.VoteType;
            }

            return 0;
        }

        private void AddPostChanges(Post originalPost, EditPostFormModel newPostData, Dictionary<string, bool> postChanges)
        {
            foreach (var kvp in postChanges)
            {
                if (kvp.Key == "HtmlContent")
                {
                    originalPost.HtmlContent = newPostData.HtmlContent;
                }
                if (kvp.Key == "Title")
                {
                    originalPost.Title = newPostData.Title;
                }
                if (kvp.Key == "CategoryId")
                {
                    originalPost.CategoryId = newPostData.CategoryId;
                }
            }
        }
    }
}
