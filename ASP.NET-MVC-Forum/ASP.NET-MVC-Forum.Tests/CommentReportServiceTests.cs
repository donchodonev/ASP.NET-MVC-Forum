namespace ASP.NET_MVC_Forum.Tests
{
    using ASP.NET_MVC_Forum.Business;
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Exceptions;
    using ASP.NET_MVC_Forum.Domain.Models.Comment;
    using ASP.NET_MVC_Forum.Infrastructure.MappingProfiles;
    using ASP.NET_MVC_Forum.Validation.Contracts;
    using ProfanityFilter;
    using AutoMapper;

    using Microsoft.EntityFrameworkCore;

    using Moq;

    using NUnit.Framework;

    using ProfanityFilter.Interfaces;

    using System.Linq;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.CommonConstants;

    internal class CommentReportServiceTests
    {
        private MapperConfiguration mapperConfiguration;
        private IMapper mapper;
        private CommentReportMappingProfile commentReportMapperProfile;
        private CommentMappingProfile commentMapperProfile;
        private ICommentReportService commentReportService;
        private DbContextOptions<ApplicationDbContext> dbContextOptions;
        private ApplicationDbContext dbContext;
        private ICommentReportRepository commentReportRepository;
        private ICommentRepository commentRepository;
        private IProfanityFilter profanityFilter;
        private Mock<ICommentValidationService> commentValidationServiceMock;
        private Mock<ICommentReportValidationService> commentReportValidationServiceMock;

        [SetUp]
        public void SetUp()
        {
            commentReportMapperProfile = new CommentReportMappingProfile();
            commentMapperProfile = new CommentMappingProfile();
            mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfiles(new Profile[] { commentReportMapperProfile, commentMapperProfile }));
            mapper = new Mapper(mapperConfiguration);
            dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("ForumDb").Options;
            dbContext = new ApplicationDbContext(dbContextOptions);
            commentReportRepository = new CommentReportRepository(dbContext);
            commentRepository = new CommentRepository(mapper, dbContext);
            profanityFilter = new ProfanityFilter();
            commentValidationServiceMock = new Mock<ICommentValidationService>();
            commentReportValidationServiceMock = new Mock<ICommentReportValidationService>();
            commentReportService = new CommentReportService(mapper, commentReportRepository, profanityFilter, commentRepository, commentReportValidationServiceMock.Object, commentValidationServiceMock.Object);
        }

        [TearDown]
        public async Task TeardownAsync()
        {
            var comments = await commentRepository.All().ToListAsync();

            var commentReports = await commentReportRepository.All().ToListAsync();

            dbContext.CommentReports.RemoveRange(commentReports);

            dbContext.Comments.RemoveRange(comments);

            await dbContext.SaveChangesAsync();
        }

        [Test]
        public void GenerateCommentReportViewModelListAsync_ShouldThrowException_WhenReportStatusIsIncorrect()
        {
            string reportStatus = "some status";

            commentReportValidationServiceMock
                .Setup(x => x.ValidateStatus(reportStatus))
                .Throws(new InvalidReportStatusException());

            Assert.ThrowsAsync<InvalidReportStatusException>(() =>
            commentReportService.GenerateCommentReportViewModelListAsync(reportStatus));
        }

        [Test]
        public async Task GenerateCommentReportViewModelListAsync_ShouldReturnCorrectCountOfActiveReports_WhenReportStatusIsSetToActive()
        {
            await AddReportsAsync(); //Adds 2 deleted and 3 active comment reports

            await commentReportService.GenerateCommentReportViewModelListAsync(ACTIVE_STATUS);

            int expectedCountOfReports = 3;
            int actualCountOfReports = await commentReportRepository
                .All()
                .Where(x => !x.IsDeleted)
                .CountAsync();

            Assert.AreEqual(expectedCountOfReports, actualCountOfReports);
        }

        [Test]
        public async Task GenerateCommentReportViewModelListAsync_ShouldReturnCorrectCountOfDeletedReports_WhenReportStatusIsSetToDeleted()
        {
            await AddReportsAsync(); //Adds 2 deleted and 3 active comment reports

            await commentReportService.GenerateCommentReportViewModelListAsync(DELETED_STATUS);

            int expectedCountOfReports = 2;
            int actualCountOfReports = await commentReportRepository
                .All()
                .Where(x => x.IsDeleted)
                .CountAsync();

            Assert.AreEqual(expectedCountOfReports, actualCountOfReports);
        }

        [Test]
        public void CensorCommentAsync_ShouldThrowException_WhenCommentIdDoesntExist()
        {
            int commentId = 1;

            commentValidationServiceMock.Setup(x => x.ValidateCommentNotNull((Comment)null))
                .Throws(new NullCommentException());

            Assert.ThrowsAsync<NullCommentException>(() => commentReportService.CensorCommentAsync(true, commentId));
        }

        [Test]
        public async Task CensorCommentAsync_ShouldReplaceBadWordsWith5Asterix_WhenCensorWithRegexIsSelected()
        {
            string badWord = "shit";

            bool withRegex = true;

            int commentId = 1;

            await commentRepository.AddCommentAsync(new RawCommentServiceModel() { Id = commentId, CommentText = badWord });

            await commentReportService.CensorCommentAsync(withRegex, commentId);

            string expectedCommentContent = "*****";

            string actualCommentContent = await commentRepository
                .GetById(commentId)
                .Select(x => x.Content)
                .FirstOrDefaultAsync();

            Assert.AreEqual(expectedCommentContent, actualCommentContent);
        }

        [Test]
        public async Task CensorCommentAsync_ShouldReplaceBadWordsWithAsterixWhichMatchesBadWordLength_WhenCensorWithoutRegexIsSelected()
        {
            string badWord = "shit";

            bool withRegex = false;

            int commentId = 1;

            await commentRepository.AddCommentAsync(new RawCommentServiceModel() { Id = commentId, CommentText = badWord });

            await commentReportService.CensorCommentAsync(withRegex, commentId);

            string expectedCommentContent = "****";

            string actualCommentContent = await commentRepository
                .GetById(commentId)
                .Select(x => x.Content)
                .FirstOrDefaultAsync();

            Assert.AreEqual(expectedCommentContent, actualCommentContent);
        }

        [Test]
        public void ReportAsync_ShouldThrowException_WhenCommentDoesntExist()
        {
            int commentId = 1;

            string reason = "some reason";

            commentValidationServiceMock
                .Setup(x => x.ValidateCommentExistsAsync(commentId))
                .ThrowsAsync(new EntityDoesNotExistException());

            Assert.ThrowsAsync<EntityDoesNotExistException>(() => commentReportService.ReportAsync(commentId, reason));
        }

        [Test]
        public async Task ReportAsync_ShouldCreateCommentReportInDatabase_WithGivenCommentIdAndReason()
        {
            int commentId = 1;

            string reason = "some reason";

            commentValidationServiceMock
                .Setup(x => x.ValidateCommentExistsAsync(commentId))
                .Returns(Task.CompletedTask);

            await commentRepository.AddCommentAsync(new RawCommentServiceModel() { Id = commentId, CommentText = "some text" });

            await commentReportService.ReportAsync(commentId, reason);

            bool reportExists = (await commentReportRepository.GetByCommentIdAsync(commentId)) != null;

            Assert.True(reportExists);
        }

        [Test]
        public void AutoGenerateCommentReportAsync_ShouldThrowException_WhenCommentDoesntExist()
        {
            int commentId = 1;

            string badWord = "shit";

            commentValidationServiceMock
                .Setup(x => x.ValidateCommentExistsAsync(commentId))
                .ThrowsAsync(new EntityDoesNotExistException());

            Assert.ThrowsAsync<EntityDoesNotExistException>(() => commentReportService.AutoGenerateCommentReportAsync(badWord, commentId));
        }

        [Test]
        public async Task AutoGenerateCommentReportAsync_ShouldNotGenerateReport_WhenNoBadWordsAreFoundInContent()
        {
            int commentId = 1;

            commentValidationServiceMock
                .Setup(x => x.ValidateCommentExistsAsync(commentId))
                .Returns(Task.CompletedTask);

            string badWord = string.Empty;

            await commentReportService.AutoGenerateCommentReportAsync(badWord, commentId);

            int expectedReportsCount = 0;

            int actualReportsCount = await commentReportRepository.All().CountAsync();

            Assert.AreEqual(expectedReportsCount, actualReportsCount);
        }

        [Test]
        public async Task AutoGenerateCommentReportAsync_ShouldGenerateReport_WhenBadWordsAreFoundInContent()
        {
            int commentId = 1;

            commentValidationServiceMock
                .Setup(x => x.ValidateCommentExistsAsync(commentId))
                .Returns(Task.CompletedTask);

            string badWord = "shit";

            await commentReportService.AutoGenerateCommentReportAsync(badWord, commentId);

            int expectedReportsCount = 1;

            int actualReportsCount = await commentReportRepository.All().CountAsync();

            Assert.AreEqual(expectedReportsCount, actualReportsCount);
        }

        [Test]
        public void DeleteAsync_ShouldThrowException_WhenReportNotFound()
        {
            int reportId = 1;

            commentReportValidationServiceMock
                .Setup(x => x.ValidateNotNull(null))
                .Throws(new CommentReportDoesNotExistException());

            Assert.ThrowsAsync<CommentReportDoesNotExistException>(() => commentReportService.DeleteAsync(reportId));
        }

        [Test]
        public async Task DeleteAsync_ShouldMarkReportAsDelete_WhenReportExists()
        {
            await AddReportsAsync();

            int reportId = 3;

            int commentId = 1;

            string reasonForReport = "some reason";

            await commentReportService.ReportAsync(commentId, reasonForReport);

            bool reportExists = await commentReportRepository.ExistsAsync(reportId);

            Assert.IsTrue(reportExists);

            await commentReportService.DeleteAsync(reportId);

            reportExists = await commentReportRepository.ExistsAsync(reportId);

            Assert.IsFalse(reportExists);
        }

        [Test]
        public void RestoreAsync_ShouldThrowException_When_ReportIsNotFoundById()
        {
            int reportId = 1;

            commentReportValidationServiceMock
                .Setup(x => x.ValidateExistsAsync(reportId))
                .Throws(new CommentReportDoesNotExistException());

            Assert.ThrowsAsync<CommentReportDoesNotExistException>(() => commentReportService.RestoreAsync(reportId));
        }

        [Test]
        public async Task RestoreAsync_Should_NOT_ThrowException_When_ReportIsExists()
        {
            await AddReportsAsync();

            int reportId = 1;

            commentReportValidationServiceMock
                .Setup(x => x.ValidateExistsAsync(reportId))
                .Returns(Task.CompletedTask);

            bool reportIsNotActive = !await commentReportRepository.ExistsAsync(reportId);

            Assert.IsTrue(reportIsNotActive);

            await commentReportService.RestoreAsync(reportId);

            reportIsNotActive = !await commentReportRepository.ExistsAsync(reportId);

            Assert.IsFalse(reportIsNotActive);
        }

        [Test]
        public void DeleteAndResolveAsync_ShouldThrowException_When_ReportIsNotFoundById()
        {
            int reportId = 1;

            commentReportValidationServiceMock
                .Setup(x => x.ValidateExistsAsync(reportId))
                .Throws(new CommentReportDoesNotExistException());

            Assert.ThrowsAsync<CommentReportDoesNotExistException>(() => commentReportService.DeleteAndResolveAsync(reportId));
        }

        [Test]
        public async Task DeleteAndResolveAsync_Should_Mark_Both_Comment_And_Report_As_Deleted()
        {
            await AddReportsAsync();

            int reportId = 3;
            int commentId = 3;

            commentReportValidationServiceMock
                .Setup(x => x.ValidateExistsAsync(reportId))
                .Returns(Task.CompletedTask);

            bool reportIsActive = await commentReportRepository.ExistsAsync(reportId);
            bool commentIsActive = await commentRepository.ExistsAsync(commentId);

            Assert.IsTrue(reportIsActive);
            Assert.IsTrue(commentIsActive);

            await commentReportService.DeleteAndResolveAsync(reportId);

            reportIsActive = await commentReportRepository.ExistsAsync(reportId);
            commentIsActive = await commentRepository.ExistsAsync(commentId);

            Assert.IsFalse(reportIsActive);
            Assert.IsFalse(commentIsActive);
        }

        private async Task AddReportsAsync()
        {
            await commentReportRepository.AddAsync(new CommentReport()
            {
                Id = 1,
                CommentId = 1,
                IsDeleted = true,
                Comment = new Comment(),
                Reason = "test"
            });
            await commentReportRepository.AddAsync(new CommentReport()
            {
                Id = 2,
                CommentId = 2,
                IsDeleted = true,
                Comment = new Comment(),
                Reason = "test"
            });
            await commentReportRepository.AddAsync(new CommentReport()
            {
                Id = 3,
                CommentId = 3,
                IsDeleted = false,
                Comment = new Comment(),
                Reason = "test"
            });
            await commentReportRepository.AddAsync(new CommentReport()
            {
                Id = 4,
                CommentId = 4,
                IsDeleted = false,
                Comment = new Comment(),
                Reason = "test"

            });
            await commentReportRepository.AddAsync(new CommentReport()
            {
                Id = 5,
                CommentId = 5,
                IsDeleted = false,
                Comment = new Comment(),
                Reason = "test"
            });
        }
    }
}
