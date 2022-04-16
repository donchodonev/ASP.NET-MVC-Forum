namespace ASP.NET_MVC_Forum.Tests.Controllers
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Models;
    using ASP.NET_MVC_Forum.Domain.Models.Post;
    using ASP.NET_MVC_Forum.Web.Controllers;

    using Microsoft.AspNetCore.Mvc;

    using Moq;

    using NUnit.Framework;

    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class HomeControllerTests
    {
        private HomeController homeController;

        private Mock<IPostService> postServiceMock;

        private Mock<ICategoryRepository> categoryRepoMock;

        [SetUp]
        public void SetUp()
        {
            postServiceMock = new Mock<IPostService>();

            categoryRepoMock = new Mock<ICategoryRepository>();

            homeController = new HomeController(postServiceMock.Object, categoryRepoMock.Object);
        }

        [Test]
        public async Task HomeController_IndexRoute_Returns_PaginatedListOfPostPreviewViewModels()
        {
            var listOfPostPreviewsAsQueryable = new List<PostPreviewViewModel>().AsQueryable();

            postServiceMock
                .Setup(x => x.GetFilteredAs<PostPreviewViewModel>(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(listOfPostPreviewsAsQueryable);

            var category = "some category";

            categoryRepoMock.Setup(x => x.GetCategoryNamesAsync())
                .ReturnsAsync(new List<string>() { category });

            var viewResult = await homeController.Index(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<string>());

            Assert.IsAssignableFrom<ViewResult>(viewResult);
            Assert.IsAssignableFrom<PaginatedList<PostPreviewViewModel>>(homeController.ViewData.Model);
        }

        [Test]
        public async Task HomeController_PrivacyRoute_ShouldReturn_PrivacyView()
        {
            Assert.IsAssignableFrom<ViewResult>(homeController.Privacy());
        }
    }
}
