namespace ASP.NET_MVC_Forum.Tests
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
        private Mock<IVoteRepository> voteRepositoryMock;
        private Mock<ICategoryRepository> categoryRepositoryMock;
        private IPostService postService;
        private IHtmlManipulator htmlManipulator;
        private IHtmlSanitizer htmlSanitizer;

        private const string userId = "some id";
        private const int postId = 1;
        private const int categoryId = 1;

        [SetUp]
        public async Task SetUpAsync()
        {
            postMapperProfile = new PostMappingProfile();

            mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(postMapperProfile));

            mapper = new Mapper(mapperConfiguration);

            dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("ForumDb")
                .Options;

            dbContext = new ApplicationDbContext(dbContextOptions);

            postRepo = new PostRepository(dbContext);

            htmlSanitizer = new HtmlSanitizer();

            htmlManipulator = new HtmlManipulator(htmlSanitizer);

            postReportServiceMock = new Mock<IPostReportService>();

            postValidationServiceMock = new Mock<IPostValidationService>();

            voteRepositoryMock = new Mock<IVoteRepository>();

            categoryRepositoryMock = new Mock<ICategoryRepository>();

            userValidationServiceMock = new Mock<IUserValidationService>();

            postService = new PostService(
                postRepo,
                postReportServiceMock.Object,
                htmlManipulator,
                voteRepositoryMock.Object,
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
            var addPostFormModel = new AddPostFormModel()
            {
                CategoryId = categoryId,
                HtmlContent = "html content",
                Title = "some title"
            };

            await postService.CreateNewAsync(addPostFormModel, userId);

            int expectedPostCount = 1;
            int actualPostCount = await postRepo.All().CountAsync();

            Assert.AreEqual(expectedPostCount, actualPostCount);
        }

        [Test]
        public async Task CreateNewAsync_Returns_AddPostResponseModel()
        {
            int responseModelId = 2; // one is already seeded in SetUpAsync method

            string title = "some title";

            string htmlContent = "html content";

            var addPostFormModel = new AddPostFormModel()
            {
                CategoryId = categoryId,
                HtmlContent = htmlContent,
                Title = "some title"
            };

            var expectedResponseModel = new AddPostResponseModel() { Title = title, Id = responseModelId };

            var actualResponseModel = await postService.CreateNewAsync(addPostFormModel, userId);

            Assert.AreEqual(expectedResponseModel.Id, actualResponseModel.Id);
            Assert.AreEqual(expectedResponseModel.Title, actualResponseModel.Title);
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

        protected async Task SeedDataAsync()
        {
            await SeedPostAsync();
            await SeedCategory();
        }

        protected Task SeedPostAsync(int categoryId = categoryId)
        {
            var post = new Post()
            {
                Id = postId,
                UserId = userId,
                CategoryId = categoryId,
            };

            return postRepo.AddPostAsync(post);
        }

        protected Task SeedCategory(int id = categoryId, string categoryName = "some name")
        {
            var category = new Category() { Id = id, Name = categoryName };

            dbContext.Categories.Add(category);

            return dbContext.SaveChangesAsync();
        }
    }
}
