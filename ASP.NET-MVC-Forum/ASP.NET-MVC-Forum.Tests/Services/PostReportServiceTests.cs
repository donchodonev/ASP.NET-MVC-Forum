namespace ASP.NET_MVC_Forum.Tests.Services
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

    using System.Linq;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.CommonConstants;

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
        private string profaneContent;
        private string postDescription;
        private string postTitle;

        [SetUp]
        public async Task SetUpAsync()
        {
            postReportMapperProfile = new PostReportMappingProfile();

            mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(postReportMapperProfile));

            mapper = new Mapper(mapperConfiguration);

            dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("ForumDb5")
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
            profaneContent = "shit";
            postDescription = "some description";
            postTitle = "some title";

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
        public void DeleteAsync_ShouldThrowException_When_PostReport_NotFound_ById()
        {
            postReportValidationService
                .Setup(x => x.ValidateReportNotNull(It.IsAny<PostReport>()))
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
        public void RestoreAsync_ShouldThrowException_When_PostReport_IsNotFound_ById()
        {
            postReportValidationService
                .Setup(x => x.ValidateReportNotNull(It.IsAny<PostReport>()))
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

        [Test]
        public void DeletePostAndResolveReportsAsync_Should_ThrowException_When_PostIsNotFound_ById()
        {
            postValidationService.Setup(x => x.ValidateNotNull(It.IsAny<Post>()))
                .Throws<PostNullReferenceException>();

            Assert.ThrowsAsync<PostNullReferenceException>(() => postReportService.DeletePostAndResolveReportsAsync(postId));
        }

        [Test]
        public async Task DeletePostAndResolveReportsAsync_Should_DeletePost_And_ResolvePostReports()
        {
            Assert.True(await postRepo.ExistsAsync(postId));
            Assert.True(await postReportRepo.ExistsAsync(reportId));

            await postReportService.DeletePostAndResolveReportsAsync(postId);

            Assert.False(await postRepo.ExistsAsync(postId));
            Assert.False(await postReportRepo.ExistsAsync(reportId));
        }

        [Test]
        public void GeneratePostReportViewModelListAsync_Should_ThrowException_When_ReportStatus_Is_Invalid()
        {
            string reportStatus = "some status";

            postReportValidationService
                .Setup(x => x.ValidateStatus(reportStatus))
                .Throws<InvalidReportStatusException>();

            Assert.ThrowsAsync<InvalidReportStatusException>(() =>
            postReportService.GeneratePostReportViewModelListAsync(reportStatus));
        }

        [Test]
        public async Task GeneratePostReportViewModelListAsync_Should_Generate_ListOfPostReportViewModel_When_ReportStatus_Is_ACTIVE()
        {
            var activeReports = await postReportService.GeneratePostReportViewModelListAsync(ACTIVE_STATUS);

            int expectedActiveReportsCount = 1;

            int actualActiveReportsCount = activeReports.Count;

            Assert.AreEqual(expectedActiveReportsCount, actualActiveReportsCount);
        }

        [Test]
        public async Task GeneratePostReportViewModelListAsync_Should_Generate_ListOfPostReportViewModel_When_ReportStatus_Is_DELETED()
        {
            await postReportService.DeleteAsync(reportId);

            var deletedReports = await postReportService.GeneratePostReportViewModelListAsync(DELETED_STATUS);

            int expectedDeletedReportsCount = 1;

            int actualDeletedReportsCount = deletedReports.Count;

            Assert.AreEqual(expectedDeletedReportsCount, actualDeletedReportsCount);
        }

        [Test]
        public void CensorAsync_ShouldThrowException_When_PostNotFound_ById()
        {
            bool withRegex = true;

            postValidationService.Setup(x => x.ValidateNotNull(It.IsAny<Post>()))
                .Throws<PostNullReferenceException>();

            Assert.ThrowsAsync<PostNullReferenceException>(() => postReportService.CensorAsync(withRegex, postId));
        }

        [Test]
        public async Task CensorAsync_ShouldCensorPost_WithRegex_When_Method_FirstParameterIs_True()
        {
            bool withRegex = true;

            var reportContent = await GetFirstReportContentAsync();

            bool isContentProfane = reportContent == "shit";

            await postReportService.CensorAsync(withRegex, postId);

            reportContent = await GetFirstReportContentAsync();

            //notice count of asterix, library replaces each word with *, while regex replaces entire word with EXACTLY 5 asterixes
            bool isContentCensoredWithRegex = reportContent == "*****";

            Assert.True(isContentProfane);
            Assert.True(isContentCensoredWithRegex);

            isContentProfane = reportContent == "shit";

            Assert.False(isContentProfane);
        }

        [Test]
        public async Task CensorAsync_ShouldCensorPost_WithCensoringLibrary_When_Method_FirstParameterIs_False()
        {
            bool withRegex = false;

            var reportContent = await GetFirstReportContentAsync();

            bool isContentProfane = reportContent == "shit";

            await postReportService.CensorAsync(withRegex, postId);

            reportContent = await GetFirstReportContentAsync();

            //notice count of asterix, library replaces each word with *, while regex replaces entire word with EXACTLY 5 asterixes
            bool isContentCensoredWithRegex = reportContent == "****";

            Assert.True(isContentProfane);
            Assert.True(isContentCensoredWithRegex);

            isContentProfane = reportContent == "shit";

            Assert.False(isContentProfane);
        }

        private async Task SeedDataAsync()
        {
            await TeardownAsync();

            await SeedPost();
            await SeedPostReport();
        }

        private Task SeedPost()
        {
            var post = new Post()
            {
                Id = postId,
                HtmlContent = profaneContent,
                ShortDescription = postDescription,
                Title = postTitle
            };

            dbContext.Posts.Add(post);

            return dbContext.SaveChangesAsync();
        }

        private Task SeedPostReport()
        {
            var postReport = new PostReport() { Id = reportId, PostId = postId, Reason = reason };

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

        private Task<string> GetFirstReportContentAsync()
        {
            return postRepo
                    .All()
                    .Select(x => x.HtmlContent)
                    .FirstOrDefaultAsync();
        }
    }
}
