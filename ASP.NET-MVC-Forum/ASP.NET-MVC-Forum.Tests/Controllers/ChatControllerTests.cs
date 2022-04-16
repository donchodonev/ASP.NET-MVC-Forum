namespace ASP.NET_MVC_Forum.Tests.Controllers
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Domain.Models.Chat;
    using ASP.NET_MVC_Forum.Web.Controllers;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Moq;

    using NUnit.Framework;

    using System.Security.Claims;
    using System.Threading.Tasks;

    public class ChatControllerTests
    {
        private ChatController chatController;
        private Mock<IChatService> chatServiceMock;

        [SetUp]
        public void SetUp()
        {
            chatServiceMock = new Mock<IChatService>();
            chatController = new ChatController(chatServiceMock.Object);
        }

        [Test]
        public async Task ChatController_SelectUser_ShouldReturnView_WithNotModel_WhenModelStateIs_Invalid()
        {
            chatController.ModelState.AddModelError("key", "error");

            var result = await chatController.SelectUser(It.IsAny<SelectUserInputModel>());

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.AreEqual(null, chatController.ViewData.Model);
        }

        [Test]
        public async Task ChatController_SelectUser_ShouldReturnView_WithModel_WhenModelStateIs_Valid()
        {
            chatServiceMock
                .Setup(x => x.GenerateChatSelectUserViewModel(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(new ChatSelectUserViewModel());

            SetupUserContext(chatController);

            var result = await chatController.SelectUser(new SelectUserInputModel() { Username = It.IsAny<string>() });

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.IsAssignableFrom<ChatSelectUserViewModel>(chatController.ViewData.Model);
        }

        private void SetupUserContext(ChatController chatController)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                            new Claim(ClaimTypes.Name, "example name"),
                            new Claim(ClaimTypes.NameIdentifier, "1"),
                            new Claim("custom-claim", "example claim value"),
            }, "mock"));


            chatController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }
    }
}
