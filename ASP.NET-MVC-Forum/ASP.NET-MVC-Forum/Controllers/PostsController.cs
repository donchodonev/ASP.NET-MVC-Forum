namespace ASP.NET_MVC_Forum.Web.Controllers
{
    using ASP.NET_MVC_Forum.Web.Models.Post;
    using ASP.NET_MVC_Forum.Web.Services.Business.Post;
    using ASP.NET_MVC_Forum.Web.Services.Business.PostReport;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using static ASP.NET_MVC_Forum.Web.Data.Constants.ClientMessage;
    using static ASP.NET_MVC_Forum.Web.Infrastructure.Extensions.ClaimsPrincipalExtensions;
    using static ASP.NET_MVC_Forum.Web.Infrastructure.Extensions.ControllerExtensions;

    public class PostsController : Controller
    {
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IPostBusinessService postBusinessService;
        private readonly IPostReportBusinessService postReportBusinessService;

        public PostsController(
            SignInManager<IdentityUser> signInManager,
            IPostBusinessService postBusinessService,
            IPostReportBusinessService postReportBusinessService)
        {
            this.signInManager = signInManager;
            this.postBusinessService = postBusinessService;
            this.postReportBusinessService = postReportBusinessService;
        }

        public async Task<IActionResult> ViewPost(int postId)
        {
            var post = await postBusinessService.GenerateViewPostModel(postId);

            if (post == null)
            {
                return this.RedirectToActionWithErrorMessage(Error.SuchAPostDoesNotExist, "Home", "Index");
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
                return this.RedirectToActionWithErrorMessage(Error.PostLengthTooSmall, "Posts", "Add");
            }

            if (await postBusinessService.PostExistsAsync(data.Title))
            {
                TempData["Title"] = data.Title;
                TempData["HtmlContent"] = data.HtmlContent;

                return this.RedirectToActionWithErrorMessage(Error.DuplicatePostName, "Posts", "Add");
            }

            var newlyCreatedPost = await postBusinessService.CreateNewAsync(data, this.User.Id());

            return RedirectToAction("ViewPost", new { postId = newlyCreatedPost.Id, postTitle = newlyCreatedPost.Title });
        }

        [Authorize]
        public async Task<IActionResult> Edit(int postId)
        {
            if (!await postBusinessService.IsUserPrivileged(postId, this.User))
            {
                return this.RedirectToActionWithErrorMessage(Error.YouAreNotTheAuthor, "Home", "Index");
            }

            var vm = postBusinessService.GenerateEditPostFormModel(postId);

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Edit([FromForm] EditPostFormModel data)
        {
            if (!await postBusinessService.IsUserPrivileged(data.PostId, this.User))
            {
                return this.RedirectToActionWithErrorMessage(Error.YouAreNotTheAuthor, "Home", "Index");
            }

            var postChanges = await postBusinessService
                .GetPostChangesAsync(data.PostId, data.HtmlContent, data.Title, data.CategoryId);

            if (postChanges.Count == 0)
            {
                return this.RedirectToActionWithErrorMessage(Error.PostRemainsUnchanged, "Posts", "Edit", new { postId = data.PostId });
            }

            await postBusinessService.Edit(data);

            return RedirectToAction("ViewPost", new { postId = data.PostId, postTitle = data.Title });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int postId, string postTitle)
        {
            if (!await postBusinessService.IsUserPrivileged(postId, this.User))
            {
                return this.RedirectToActionWithErrorMessage(Error.YouAreNotTheAuthor, "Home", "Index");
            }

            if (!await postBusinessService.PostExistsAsync(postId))
            {
                BadRequest();
            }

            await postBusinessService.Delete(postId);

            return this.RedirectToActionWithSuccessMessage(Success.PostSuccessfullyDeleted, "Home", "Index");
        }

        public async Task<IActionResult> Report([FromForm] string content, int postId)
        {
            if (!await postBusinessService.PostExistsAsync(postId))
            {
                return BadRequest();
            }

            await postReportBusinessService.ReportAsync(postId, content);

            return this.RedirectToActionWithSuccessMessage(Success.ReportThankYouMessage, "Home", "Index");
        }
    }
}
