namespace ASP.NET_MVC_Forum.Tests
{
    using ASP.NET_MVC_Forum.Business;
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Exceptions;
    using ASP.NET_MVC_Forum.Infrastructure.MappingProfiles;
    using ASP.NET_MVC_Forum.Validation.Contracts;

    using AutoMapper;

    using Microsoft.EntityFrameworkCore;

    using Moq;

    using NUnit.Framework;

    using ProfanityFilter;

    using System.Threading.Tasks;

    public class PostReportServiceTests
    {
        private MapperConfiguration mapperConfiguration;
        private IMapper mapper;
        private PostReportMappingProfile postReportMapperProfile;
        private DbContextOptions<ApplicationDbContext> dbContextOptions;
        private ApplicationDbContext dbContext;
        private IPostRepository postRepo;
        private IPostReportRepository postReportRepo;
        private IPostReportService postReportService;
        private Mock<IPostValidationService> postValidationService;
        private Mock<IPostReportValidationService> postReportValidationService;
        private ProfanityFilter profanityFilter;

        private int postId;
        private string reason;
        private int reportId;

        [SetUp]
        public async Task SetUpAsync()
        {
            postReportMapperProfile = new PostReportMappingProfile();

            mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(postReportMapperProfile));

            mapper = new Mapper(mapperConfiguration);

            dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("ForumDb")
                .Options;

            dbContext = new ApplicationDbContext(dbContextOptions);

            postRepo = new PostRepository(dbContext);

            postReportRepo = new PostReportRepository(dbContext);

            profanityFilter = new ProfanityFilter();

            postValidationService = new Mock<IPostValidationService>();

            postReportValidationService = new Mock<IPostReportValidationService>();

            postReportService = new PostReportService(
                postReportRepo,
                postRepo,
                mapper,
                postValidationService.Object,
                postReportValidationService.Object,
                profanityFilter);

            postId = 1;
            reason = "some reason";
            reportId = 1;

            await SeedDataAsync();
        }

        [Test]
        public void ReportAsync_ShouldThrowException_When_PostBeingReported_Does_NOT_Exist()
        {
            postValidationService
                .Setup(x => x.ValidatePostExistsAsync(postId))
                .Throws(new EntityDoesNotExistException());

            Assert.ThrowsAsync<EntityDoesNotExistException>(() => postReportService.ReportAsync(postId, reason));
        }

        [Test]
        public async Task ReportAsync_ShouldCreate_PostReport_WhenPostExists()
        {
            await TeardownAsync();

            int expectedReportsCount = 1;

            await postReportService.ReportAsync(postId, reason);

            int actualReportsCount = await postReportRepo.All().CountAsync();

            Assert.AreEqual(expectedReportsCount, actualReportsCount);
        }


        [Test]
        public async Task DeleteAsync_ShouldThrowException_When_PostReport_NotFound_ById()
        {
            await TeardownAsync();

            postReportValidationService
                .Setup(x => x.ValidateReportNotNull(null))
                .Throws<PostReportDoesNotExistException>();

            Assert.ThrowsAsync<PostReportDoesNotExistException>(() => postReportService.DeleteAsync(reportId));
        }

        [Test]
        public async Task DeleteAsync_ShouldMarkPostReportAsDeleted_When_PostReportExists()
        {
            Assert.True(await postReportRepo.ExistsAsync(reportId));

            await postReportService.DeleteAsync(reportId);

            Assert.False(await postReportRepo.ExistsAsync(reportId));
        }

        [Test]
        public async Task RestoreAsync_ShouldThrowException_When_PostReport_IsNotFound_ById()
        {
            await TeardownAsync();

            postReportValidationService
                .Setup(x => x.ValidateReportNotNull(null))
                .Throws<PostReportDoesNotExistException>();

            Assert.ThrowsAsync<PostReportDoesNotExistException>(() => postReportService.RestoreAsync(reportId));
        }

        [Test]
        public async Task RestoreAsync_MarkPostReport_As_UnDeleted()
        {
            await postReportService.DeleteAsync(reportId);

            Assert.False(await postReportRepo.ExistsAsync(reportId));

            await postReportService.RestoreAsync(reportId);

            Assert.True(await postReportRepo.ExistsAsync(reportId));
        }

        [Test]
        public async Task AutoGeneratePostReportAsync_Should_TriggerReportAsyncMethod_WhenProfanitiesAreFound()
        {
            string profaneTitle = "shit";
            string profaneDescription = "fuck";

            int reportCountBefore = await postReportRepo.All().CountAsync();

            await postReportService.AutoGeneratePostReportAsync(profaneTitle, profaneDescription, postId);

            int reportCountAfter = await postReportRepo.All().CountAsync();

            Assert.AreNotEqual(reportCountBefore, reportCountAfter);
            Assert.True(reportCountBefore == (reportCountAfter - 1));
        }

        [Test]
        public async Task AutoGeneratePostReportAsync_Should_NOT_TriggerReportAsyncMethod_WhenProfanitiesAre_NotFound()
        {
            string properTitle = "Hi";
            string properDescription = "there";

            int reportCountBefore = await postReportRepo.All().CountAsync();

            await postReportService.AutoGeneratePostReportAsync(properTitle, properDescription, postId);

            int reportCountAfter = await postReportRepo.All().CountAsync();

            Assert.AreEqual(reportCountBefore, reportCountAfter);
        }

        private async Task SeedDataAsync()
        {
            await TeardownAsync();

            await SeedPost();
            await SeedPostReport();
        }

        private Task SeedPost()
        {
            var post = new Post() { Id = postId };

            dbContext.Posts.Add(post);

            return dbContext.SaveChangesAsync();
        }

        private Task SeedPostReport()
        {
            var postReport = new PostReport() { Id = reportId, PostId = postId };

            dbContext.PostReports.Add(postReport);

            return dbContext.SaveChangesAsync();
        }

        private async Task TeardownAsync()
        {
            var posts = await dbContext.Posts.ToListAsync();
            var postReports = await dbContext.PostReports.ToListAsync();

            dbContext.RemoveRange(posts);
            dbContext.RemoveRange(postReports);

            await dbContext.SaveChangesAsync();
        }
    }
}
