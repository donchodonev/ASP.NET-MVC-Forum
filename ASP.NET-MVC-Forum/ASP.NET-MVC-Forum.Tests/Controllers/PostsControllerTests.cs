namespace ASP.NET_MVC_Forum.Tests.Controllers
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Domain.Models.Post;
    using ASP.NET_MVC_Forum.Tests.Fakes;
    using ASP.NET_MVC_Forum.Web.Controllers;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

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

        [SetUp]
        public void SetUp()
        {
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

            postsController = new PostsController(
                signInManagerFake,
                postServiceMock.Object,
                postReportServiceMock.Object,
                voteServiceMock.Object);

            SetupUserContext(postsController);

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

            SetupUserContext(postsController);

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

        private void SetupUserContext(PostsController postsController)
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

            postsController.ControllerContext.HttpContext.User = user;
        }
    }
}
