namespace ASP.NET_MVC_Forum.Tests.Controllers
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Web.Controllers;

    using Moq;

    using NUnit.Framework;

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
    }
}
