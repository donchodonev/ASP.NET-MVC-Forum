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

    using ProfanityFilter.Interfaces;

    using System.Linq;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.CommonConstants;

    internal class CommentReportServiceTests
    {
        private MapperConfiguration mapperConfiguration;
        private IMapper mapper;
        private CommentReportMappingProfile commentReportMapperProfile;
        private ICommentReportService commentReportService;
        private DbContextOptions<ApplicationDbContext> dbContextOptions;
        private ApplicationDbContext dbContext;
        private ICommentReportRepository commentReportRepository;
        private ICommentRepository commentRepository;
        private Mock<IProfanityFilter> profanityFilterMock;
        private Mock<ICommentValidationService> commentValidationServiceMock;
        private Mock<ICommentReportValidationService> commentReportValidationServiceMock;

        [SetUp]
        public void SetUp()
        {
            commentReportMapperProfile = new CommentReportMappingProfile();
            mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(commentReportMapperProfile));
            mapper = new Mapper(mapperConfiguration);
            dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("ForumDb").Options;
            dbContext = new ApplicationDbContext(dbContextOptions);
            commentReportRepository = new CommentReportRepository(dbContext);
            commentRepository = new CommentRepository(mapper, dbContext);
            profanityFilterMock = new Mock<IProfanityFilter>();
            commentValidationServiceMock = new Mock<ICommentValidationService>();
            commentReportValidationServiceMock = new Mock<ICommentReportValidationService>();
            commentReportService = new CommentReportService(mapper, commentReportRepository, profanityFilterMock.Object, commentRepository, commentReportValidationServiceMock.Object, commentValidationServiceMock.Object);
        }

        [TearDown]
        public async Task TeardownAsync()
        {
            var reports = await commentReportRepository.All().ToListAsync();

            dbContext.CommentReports.RemoveRange(reports);

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

        private async Task AddReportsAsync()
        {
            await commentReportRepository.AddAsync(new CommentReport() { CommentId = 1, IsDeleted = true });
            await commentReportRepository.AddAsync(new CommentReport() { CommentId = 2, IsDeleted = true });
            await commentReportRepository.AddAsync(new CommentReport() { CommentId = 3, IsDeleted = false });
            await commentReportRepository.AddAsync(new CommentReport() { CommentId = 4, IsDeleted = false });
            await commentReportRepository.AddAsync(new CommentReport() { CommentId = 5, IsDeleted = false });
        }
    }
}
