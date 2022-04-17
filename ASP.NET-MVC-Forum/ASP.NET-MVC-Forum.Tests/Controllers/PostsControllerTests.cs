namespace ASP.NET_MVC_Forum.Tests.Controllers
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Domain.Models.Post;
    using ASP.NET_MVC_Forum.Tests.Fakes;
    using ASP.NET_MVC_Forum.Web.Controllers;
    using ASP.NET_MVC_Forum.Web.Services.Models.Post;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    using Moq;

    using NUnit.Framework;

    using System.Security.Claims;
    using System.Threading.Tasks;

    public class PostsControllerTests
    {
        private PostsController postsController;

        private FakeSignInManager signInManagerFake;

        private Mock<IPostService> postServiceMock;

        private Mock<IPostReportService> postReportServiceMock;

        private Mock<IVoteService> voteServiceMock;

        private Mock<ITempDataProvider> tempDataProviderMock;

        [SetUp]
        public void SetUp()
        {
            tempDataProviderMock = new Mock<ITempDataProvider>();

            postServiceMock = new Mock<IPostService>();

            postReportServiceMock = new Mock<IPostReportService>();

            voteServiceMock = new Mock<IVoteService>();

            postsController = new PostsController(
                signInManagerFake,
                postServiceMock.Object,
                postReportServiceMock.Object,
                voteServiceMock.Object);
        }

        [Test]
        public async Task ViewPostAction_ShouldCallVoteServiceToGetUserVote_WhenUserIsSignedIn()
        {
            signInManagerFake = new FakeSignInManager(true);

            tempDataProviderMock = new Mock<ITempDataProvider>();

            postsController = new PostsController(
                signInManagerFake,
                postServiceMock.Object,
                postReportServiceMock.Object,
                voteServiceMock.Object);

            SetupContext(postsController);

            voteServiceMock
                .Setup(x => x.InjectUserLastVoteType(It.IsAny<ViewPostViewModel>(), It.IsAny<string>()));

            postServiceMock
                .Setup(x => x.GetPostByIdAs<ViewPostViewModel>(It.IsAny<int>()))
                .ReturnsAsync(new ViewPostViewModel());

            int countBeforeIsSignedInCheck = voteServiceMock.Invocations.Count;

            var result = await postsController.ViewPost(It.IsAny<int>());

            int countAfterIsSignedInCheck = voteServiceMock.Invocations.Count;

            Assert.Greater(countAfterIsSignedInCheck, countBeforeIsSignedInCheck);

            Assert.IsAssignableFrom<ViewResult>(result);

            Assert.IsAssignableFrom<ViewPostViewModel>(postsController.ViewData.Model);
        }

        [Test]
        public async Task ViewPostAction_Should_NOT_CallVoteServiceToGetUserVote_WhenUserIs_NOT_SignedIn()
        {
            signInManagerFake = new FakeSignInManager(false);

            postsController = new PostsController(
                signInManagerFake,
                postServiceMock.Object,
                postReportServiceMock.Object,
                voteServiceMock.Object);

            SetupContext(postsController);

            voteServiceMock
                .Setup(x => x.InjectUserLastVoteType(It.IsAny<ViewPostViewModel>(), It.IsAny<string>()));

            postServiceMock
                .Setup(x => x.GetPostByIdAs<ViewPostViewModel>(It.IsAny<int>()))
                .ReturnsAsync(new ViewPostViewModel());

            int countBeforeIsSignedInCheck = voteServiceMock.Invocations.Count;

            var result = await postsController.ViewPost(It.IsAny<int>());

            int countAfterIsSignedInCheck = voteServiceMock.Invocations.Count;

            Assert.AreEqual(countBeforeIsSignedInCheck, countAfterIsSignedInCheck);

            Assert.IsAssignableFrom<ViewResult>(result);

            Assert.IsAssignableFrom<ViewPostViewModel>(postsController.ViewData.Model);
        }

        [Test]
        public async Task Add_Get_ReturnsView()
        {
            postServiceMock
                .Setup(x => x.GenerateAddPostFormModelAsync())
                .ReturnsAsync(new AddPostFormModel());

            var result = await postsController.Add(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>());

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.IsAssignableFrom<AddPostFormModel>(postsController.ViewData.Model);
        }

        [Test]
        public async Task Add_Post_ReturnsRedirectToActionWithError_WhenModelStateIs_Invalid()
        {
            SetupContext(postsController);

            postsController.ModelState.AddModelError("key", "error");

            var result = await postsController.Add(new AddPostFormModel() { CategoryId = 1, Title = "title", HtmlContent = "" });

            Assert.IsAssignableFrom<RedirectToActionResult>(result);
        }

        [Test]
        public async Task Add_Post__ShouldCreateNewPost_And_ReturnRedirectToAction_WhenModelStateIs_Valid()
        {
            postServiceMock
                .Setup(x => x.CreateNewAsync(
                    It.IsAny<AddPostFormModel>(),
                    It.IsAny<string>()))
                .ReturnsAsync(new AddPostResponseModel());

            var invocationsBeforeCallingController = postServiceMock.Invocations.Count;

            SetupContext(postsController);

            var result = await postsController.Add(new AddPostFormModel() { CategoryId = 1, Title = "title", HtmlContent = "" });

            var invocationsAfterCallingController = postServiceMock.Invocations.Count;

            Assert.IsAssignableFrom<RedirectToActionResult>(result);
            Assert.Greater(invocationsAfterCallingController, invocationsBeforeCallingController);
        }

        [Test]
        public async Task Edit_Get_ShouldReturnView()
        {
            SetupContext(postsController);

            postServiceMock
                .Setup(x => x.GenerateEditPostFormModelAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(It.IsAny<EditPostFormModel>());

            var result = await postsController.Edit(It.IsAny<int>());

            Assert.IsAssignableFrom<ViewResult>(result);
        }

        private void SetupContext(PostsController postsController)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                            new Claim(ClaimTypes.Name, "example name"),
                            new Claim(ClaimTypes.NameIdentifier, "1"),
                            new Claim("custom-claim", "example claim value"),
            }, "mock"));

            postsController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(),
            };

            var httpContext = postsController.ControllerContext.HttpContext;

            httpContext.User = user;

            postsController.TempData = new TempDataDictionary(httpContext, tempDataProviderMock.Object);
        }
    }
}
