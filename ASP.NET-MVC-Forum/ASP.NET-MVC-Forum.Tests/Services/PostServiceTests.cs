namespace ASP.NET_MVC_Forum.Tests.Services
{
    using ASP.NET_MVC_Forum.Business;
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Exceptions;
    using ASP.NET_MVC_Forum.Domain.Models.Post;
    using ASP.NET_MVC_Forum.Infrastructure;
    using ASP.NET_MVC_Forum.Infrastructure.MappingProfiles;
    using ASP.NET_MVC_Forum.Validation.Contracts;
    using ASP.NET_MVC_Forum.Web.Services.Models.Post;

    using AutoMapper;

    using Ganss.XSS;

    using Microsoft.EntityFrameworkCore;

    using Moq;

    using NUnit.Framework;

    using System.Linq;
    using System.Threading.Tasks;

    public class PostServiceTests
    {
        private MapperConfiguration mapperConfiguration;
        private IMapper mapper;
        private PostMappingProfile postMapperProfile;
        private DbContextOptions<ApplicationDbContext> dbContextOptions;
        private ApplicationDbContext dbContext;
        private IPostRepository postRepo;
        private Mock<IPostValidationService> postValidationServiceMock;
        private Mock<IUserValidationService> userValidationServiceMock;
        private Mock<IPostReportService> postReportServiceMock;
        private Mock<ICategoryRepository> categoryRepositoryMock;
        private IPostService postService;
        private IHtmlManipulator htmlManipulator;
        private IHtmlSanitizer htmlSanitizer;

        private const string userId = "some id";
        private const int postId = 1;
        private const int categoryId = 1;
        private const string htmlContent = "some html content";
        private const string changedHtmlContent = "some html content changed";
        private const string title = "some title";
        private const string changedTitle = "some title changed";
        private const string description = "some description";

        [SetUp]
        public async Task SetUpAsync()
        {
            postMapperProfile = new PostMappingProfile();

            mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(postMapperProfile));

            mapper = new Mapper(mapperConfiguration);

            dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("ForumDb6")
                .Options;

            dbContext = new ApplicationDbContext(dbContextOptions);

            postRepo = new PostRepository(dbContext);

            htmlSanitizer = new HtmlSanitizer();

            htmlManipulator = new HtmlManipulator(htmlSanitizer);

            postReportServiceMock = new Mock<IPostReportService>();

            postValidationServiceMock = new Mock<IPostValidationService>();

            categoryRepositoryMock = new Mock<ICategoryRepository>();

            userValidationServiceMock = new Mock<IUserValidationService>();

            postService = new PostService(
                postRepo,
                postReportServiceMock.Object,
                htmlManipulator,
                categoryRepositoryMock.Object,
                mapper,
                postValidationServiceMock.Object,
                userValidationServiceMock.Object);
        }

        [TearDown]
        public async Task TeardownAsync()
        {
            var posts = await dbContext.Posts.ToListAsync();
            var categories = await dbContext.Categories.ToListAsync();
            var users = await dbContext.Users.ToListAsync();

            dbContext.Posts.RemoveRange(posts);
            dbContext.Categories.RemoveRange(categories);
            dbContext.Users.RemoveRange(users);

            await dbContext.SaveChangesAsync();
        }

        [Test]
        public void CreateNewAsync_ShouldThrowExceptio_WhenPostTitle_IsDuplicate()
        {
            postValidationServiceMock
                .Setup(x => x.ValidateTitleNotDuplicateAsync(It.IsAny<string>()))
                .Throws<DuplicatePostTitleException>();

            Assert.ThrowsAsync<DuplicatePostTitleException>(() =>
            postService.CreateNewAsync(It.IsAny<AddPostFormModel>(), userId));
        }

        [Test]
        public void CreateNewAsync_ShouldThrowExceptio_WhenUserIsNotFound_ById()
        {
            userValidationServiceMock
                .Setup(x => x.ValidateUserExistsByIdAsync(It.IsAny<string>()))
                .Throws<NullUserException>();


            Assert.ThrowsAsync<NullUserException>(() =>
            postService.CreateNewAsync(It.IsAny<AddPostFormModel>(), userId));
        }

        [Test]
        public async Task CreateNewAsync_Creates_A_New_Post()
        {
            await TeardownAsync();

            var addPostFormModel = new AddPostFormModel()
            {
                CategoryId = categoryId,
                HtmlContent = htmlContent,
                Title = title
            };

            await postService.CreateNewAsync(addPostFormModel, userId);

            int expectedPostCount = 1;
            int actualPostCount = await postRepo.All().CountAsync();

            Assert.AreEqual(expectedPostCount, actualPostCount);
        }

        [Test]
        public async Task CreateNewAsync_Returns_AddPostResponseModel()
        {
            var addPostFormModel = new AddPostFormModel()
            {
                CategoryId = categoryId,
                HtmlContent = htmlContent,
                Title = title
            };

            var actualResponseModel = await postService.CreateNewAsync(addPostFormModel, userId);

            var expectedResponseModelId = await dbContext
                .Posts
                .Select(x => x.Id)
                .LastOrDefaultAsync();

            Assert.AreEqual(expectedResponseModelId, actualResponseModel.Id);
            Assert.AreEqual(title, actualResponseModel.Title);
        }

        [Test]
        public void DeleteAsync_ShouldThrowException_WhenPostNotFound_ById()
        {
            postValidationServiceMock
                .Setup(x => x.ValidateNotNull(It.IsAny<Post>()))
                .Throws<PostNullReferenceException>();

            Assert.ThrowsAsync<PostNullReferenceException>(() =>
                postService.DeleteAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>()));
        }

        [Test]
        public void DeleteAsync_ShouldThrowException_UserIsNotPrivileged()
        {
            userValidationServiceMock
                .Setup(x => x.ValidateUserIsPrivilegedAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>()))
                .Throws<InsufficientPrivilegeException>();

            Assert.ThrowsAsync<InsufficientPrivilegeException>(() =>
                postService.DeleteAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>()));
        }

        [Test]
        public async Task DeleteAsync_ShouldDeletedPost()
        {
            await SeedPostAsync();

            Assert.IsTrue(await postRepo.ExistsAsync(postId));

            await postService.DeleteAsync(postId, userId, true);

            Assert.IsFalse(await postRepo.ExistsAsync(postId));
        }

        [Test]
        public void EditAsync_ShouldThrowException_When_UserIsNotPrivileged()
        {
            var editPostFormModel = new EditPostFormModel() { PostId = postId };

            postValidationServiceMock
                .Setup(x => x.ValidateNotNull(It.IsAny<Post>()))
                .Throws<InsufficientPrivilegeException>();

            Assert.ThrowsAsync<InsufficientPrivilegeException>(() =>
            postService.EditAsync(
                editPostFormModel,
                It.IsAny<string>(),
                It.IsAny<bool>()));
        }

        [Test]
        public void EditAsync_ShouldThrowException_WhenPostNotFound_ById()
        {
            var editPostFormModel = new EditPostFormModel() { PostId = postId };

            postValidationServiceMock
                .Setup(x => x.ValidateNotNull(It.IsAny<Post>()))
                .Throws<PostNullReferenceException>();

            Assert.ThrowsAsync<PostNullReferenceException>(() =>
            postService.EditAsync(
                editPostFormModel,
                It.IsAny<string>(),
                It.IsAny<bool>()));
        }

        [Test]
        public void EditAsync_ShouldThrowException_WhenPost_HasNotChanged()
        {
            var editPostFormModel = new EditPostFormModel() { PostId = postId };

            postValidationServiceMock
                .Setup(x => x.ValidatePostChangedAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Throws<NoUpdatesMadeException>();

            Assert.ThrowsAsync<NoUpdatesMadeException>(() =>
            postService.EditAsync(
                editPostFormModel,
                It.IsAny<string>(),
                It.IsAny<bool>()));
        }

        [Test]
        public async Task EditAsync_ShouldEditPost()
        {
            await SeedPostAsync();

            var originalPost = await postRepo
                .All()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == 1);

            bool isAdminOrModerator = true;

            var editPostFormModel = new EditPostFormModel()
            {
                PostId = postId,
                CategoryId = 2,
                HtmlContent = changedHtmlContent,
                Title = changedTitle
            };

            await postService.EditAsync(editPostFormModel, userId, isAdminOrModerator);

            var editedPost = await postRepo.GetByIdAsync(postId);

            Assert.AreNotEqual(originalPost.CategoryId, editedPost.CategoryId);
            Assert.AreNotEqual(originalPost.HtmlContent, editedPost.HtmlContent);
            Assert.AreNotEqual(originalPost.Title, editedPost.Title);
            Assert.AreNotEqual(originalPost.ShortDescription, editedPost.ShortDescription);
        }

        [Test]
        public async Task GenerateAddPostFormModelAsync_ShouldReturn_AddPostFormModel()
        {
            var categoryIdAndNameModelArray = new CategoryIdAndNameViewModel[] { new CategoryIdAndNameViewModel() };

            categoryRepositoryMock.Setup(x =>
            x.GetCategoryIdAndNameCombinationsAsync())
                .ReturnsAsync(categoryIdAndNameModelArray);

            await SeedCategoryAsync();

            var model = await postService.GenerateAddPostFormModelAsync();

            Assert.NotNull(model);
            Assert.IsTrue(model.Categories.Count() == 1);
            Assert.IsInstanceOf(typeof(AddPostFormModel), model);
        }

        [Test]
        public void GenerateEditPostFormModelAsync_ThrowsException_When_UserIs_NOT_Privileged()
        {
            userValidationServiceMock
            .Setup(x => x.ValidateUserIsPrivilegedAsync(
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
            .Throws<InsufficientPrivilegeException>();

            Assert.ThrowsAsync<InsufficientPrivilegeException>(() =>
            postService.GenerateEditPostFormModelAsync(
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<bool>()));
        }

        [Test]
        public async Task GenerateEditPostFormModelAsync_Returns_EditPostFormModel()
        {
            await SeedCategoryAsync();
            await SeedPostAsync();

            var categoryIdAndNameModelArray = new CategoryIdAndNameViewModel[] { new CategoryIdAndNameViewModel()};

            categoryRepositoryMock.Setup(x =>
            x.GetCategoryIdAndNameCombinationsAsync())
                .ReturnsAsync(categoryIdAndNameModelArray);

            var model = await postService.GenerateEditPostFormModelAsync(
                postId,
                It.IsAny<string>(),
                It.IsAny<bool>());

            Assert.NotNull(model);
            Assert.IsTrue(model.Categories.Count() == 1);
            Assert.IsInstanceOf(typeof(EditPostFormModel), model);
        }
        protected async Task SeedDataAsync()
        {
            await SeedPostAsync();
            await SeedCategoryAsync();
        }

        protected Task SeedPostAsync(int categoryId = categoryId)
        {
            var post = new Post()
            {
                Id = postId,
                UserId = userId,
                CategoryId = categoryId,
                HtmlContent = htmlContent,
                ShortDescription = description
            };

            return postRepo.AddPostAsync(post);
        }

        protected Task SeedCategoryAsync(int id = categoryId, string categoryName = "some name")
        {
            var category = new Category() { Id = id, Name = categoryName };

            dbContext.Categories.Add(category);

            return dbContext.SaveChangesAsync();
        }
    }
}
