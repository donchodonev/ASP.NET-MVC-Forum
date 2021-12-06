namespace ASP.NET_MVC_Forum.Controllers
{
    using ASP.NET_MVC_Forum.Models.Post;
    using ASP.NET_MVC_Forum.Services.Business.Post;
    using ASP.NET_MVC_Forum.Services.Business.PostReport;
    using ASP.NET_MVC_Forum.Services.Data.Post;
    using ASP.NET_MVC_Forum.Services.User;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using static ASP.NET_MVC_Forum.Data.Constants.ClientMessage.Error;
    using static ASP.NET_MVC_Forum.Data.Constants.ClientMessage.Success;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ClaimsPrincipalExtensions;
    using static ASP.NET_MVC_Forum.Infrastructure.Extensions.ControllerExtensions;

    public class PostsController : Controller
    {
        private readonly IUserService userService;
        private readonly IPostDataService postDataService;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IPostBusinessService postBusinessService;
        private readonly IPostReportBusinessService postReportBusinessService;

        public PostsController(
            IUserService userService,
            IPostDataService postDataService,
            SignInManager<IdentityUser> signInManager,
            IPostBusinessService postBusinessService,
            IPostReportBusinessService postReportBusinessService)
        {
            this.userService = userService;
            this.postDataService = postDataService;
            this.signInManager = signInManager;
            this.postBusinessService = postBusinessService;
            this.postReportBusinessService = postReportBusinessService;
        }

        public async Task<IActionResult> ViewPost(int postId)
        {
            var post = await postBusinessService.GenerateViewPostModel(postId);

            if (post == null)
            {
                return this.RedirectToActionWithErrorMessage(SuchAPostDoesNotExist, "Home", "Index");
            }

            if (signInManager.IsSignedIn(this.User))
            {
                await postBusinessService.InjectUserLastVoteType(post, this.User.Id());
            }

            return View(post);
        }

        [Authorize]
        public IActionResult Add()
        {
            var addPostFormModel = postBusinessService.GeneratedAddPostFormModel();

            if (TempData.ContainsKey("HtmlContent"))
            {
                addPostFormModel.HtmlContent = TempData["HtmlContent"].ToString();
            }
            if (TempData.ContainsKey("Title"))
            {
                addPostFormModel.Title = TempData["Title"].ToString();
            }

            return View(addPostFormModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromForm] AddPostFormModel data)
        {
            if (!ModelState.IsValid)
            {
                TempData["Title"] = data.Title;
                return this.RedirectToActionWithErrorMessage(PostLengthTooSmall, "Posts", "Add");
            }

            if (await postDataService.PostExistsAsync(data.Title))
            {
                TempData["Title"] = data.Title;
                TempData["HtmlContent"] = data.HtmlContent;

                var errorMessage = $"A post with the title \"{data.Title}\" already exists";

                return this.RedirectToActionWithErrorMessage(errorMessage, "Posts", "Add");
            }

            var newlyCreatedPost = await postBusinessService.CreateNewAsync(data, this.User.Id());

            return RedirectToAction("ViewPost", new { postId = newlyCreatedPost.Id, postTitle = newlyCreatedPost.Title });
        }

        [Authorize]
        public async Task<IActionResult> Edit(int postId)
        {
            var verificationResult = await IsUserPrivileged(postId);

            if (verificationResult != null)
            {
                return this.RedirectToActionWithErrorMessage(YouAreNotTheAuthor, "Home", "Index");
            }

            var vm = postBusinessService.GenerateEditPostFormModel(postId);

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Edit([FromForm] EditPostFormModel data)
        {
            var verificationResult = await IsUserPrivileged(data.PostId);

            if (verificationResult != null)
            {
                return verificationResult;
            }

            var postChanges = await postBusinessService
                .GetPostChangesAsync(data.PostId, data.HtmlContent, data.Title, data.CategoryId);

            if (postChanges.Count == 0)
            {
                return this.RedirectToActionWithErrorMessage(PostRemainsUnchanged, "Posts", "Edit", new { postId = data.PostId });
            }

            await postBusinessService.Edit(data);

            return RedirectToAction("ViewPost", new { postId = data.PostId, postTitle = data.Title });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int postId, string postTitle)
        {
            var verificationResult = await IsUserPrivileged(postId);

            if (verificationResult != null)
            {
                return verificationResult;
            }

            var isPostDeleted = await postDataService.IsPostDeleted(postId, postTitle);

            if (!isPostDeleted.HasValue)
            {
                BadRequest();
            }

            await postBusinessService.Delete(postId);

            return this.RedirectToActionWithSuccessMessage(PostSuccessfullyDeleted, "Home", "Index");
        }

        public async Task<IActionResult> Report([FromForm] string content, int postId)
        {
            if (!await postDataService.PostExistsAsync(postId))
            {
                return BadRequest();
            }

            await postReportBusinessService.ReportAsync(postId, content);

            return this.RedirectToActionWithSuccessMessage(ReportThankYouMessage, "Home", "Index");
        }

        private async Task<ActionResult> IsUserPrivileged(int postId)
        {
            var userId = await userService.GetBaseUserIdAsync(User.Id());

            if (!await postBusinessService.IsAuthor(userId, postId) && !this.User.IsAdminOrModerator())
            {
                return this.RedirectToActionWithErrorMessage(YouAreNotTheAuthor, "Home", "Index");
            }

            return null;
        }
    }
}
