namespace ASP.NET_MVC_Forum.Tests.Controllers
{
    using ASP.NET_MVC_Forum.Web.Controllers;

    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    using Moq;

    using NUnit.Framework;

    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.ClientMessage.MessageType;

    public class ErrorControllerTests
    {
        private ErrorController errorController;
        private Mock<HttpContext> httpContextMock;
        private Mock<IExceptionHandlerPathFeature> exceptionHandlerPathFeatureMock;
        private Mock<ITempDataProvider> tempDataProviderMock;

        [SetUp]
        public void SetUp()
        {
            errorController = new ErrorController();
            httpContextMock = new Mock<HttpContext>();
            exceptionHandlerPathFeatureMock = new Mock<IExceptionHandlerPathFeature>();
            tempDataProviderMock = new Mock<ITempDataProvider>();
        }

        [Test]
        public async Task ErrorController_RedirectsTo_HomeController_IndexAction_WithTempData_ErrorMessage()
        {
            SetupContext(errorController);

            var result = await errorController.Show();

            errorController.TempData[ERROR_MESSAGE] = "test";

            bool containsErrorMessage = errorController.TempData[ERROR_MESSAGE].ToString().Length > 0;

            Assert.IsTrue(containsErrorMessage);
            Assert.IsAssignableFrom<RedirectToActionResult>(result);
        }

        private void SetupContext(ErrorController errorController)
        {
            exceptionHandlerPathFeatureMock
                .Setup(x => x.Error.Message)
                .Returns("some error");

            httpContextMock
                .Setup(x => x.Features.Get<IExceptionHandlerPathFeature>())
                .Returns(exceptionHandlerPathFeatureMock.Object);

            errorController.ControllerContext.HttpContext = httpContextMock.Object;

            errorController.TempData = new TempDataDictionary(httpContextMock.Object,tempDataProviderMock.Object);
        }
    }
}
