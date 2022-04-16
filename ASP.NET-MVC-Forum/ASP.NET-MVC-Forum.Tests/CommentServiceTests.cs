namespace ASP.NET_MVC_Forum.Tests
{
    using ASP.NET_MVC_Forum.Business;
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Infrastructure.MappingProfiles;
    using ASP.NET_MVC_Forum.Validation.Contracts;

    using AutoMapper;

    using Microsoft.EntityFrameworkCore;

    using Moq;

    using NUnit.Framework;

    using System.Threading.Tasks;

    public class CommentServiceTests
    {
        private MapperConfiguration mapperConfiguration;
        private IMapper mapper;
        private DbContextOptions<ApplicationDbContext> dbContextOptions;
        private ApplicationDbContext dbContext;
        private ICommentRepository commentRepository;
        private Mock<IPostValidationService> postValidationServiceMock;
        private Mock<ICommentReportService> commentReportServiceMock;
        private Mock<ICommentValidationService> commentValidationServiceMock;
        private ICommentService commentService;

        [SetUp]
        public void SetUp()
        {
            mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(new CommentMappingProfile()));
            mapper = new Mapper(mapperConfiguration);
            dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("ForumDb").Options;
            dbContext = new ApplicationDbContext(dbContextOptions);
            commentRepository = new CommentRepository(mapper,dbContext);
            postValidationServiceMock = new Mock<IPostValidationService>();
            commentReportServiceMock = new Mock<ICommentReportService>();
            commentValidationServiceMock = new Mock<ICommentValidationService>();
            commentService = new CommentService(
                commentRepository,
                mapper,
                postValidationServiceMock.Object,
                commentReportServiceMock.Object,
                commentValidationServiceMock.Object);
        }

        private async Task SeedTestData()
        {
            commentService.
        }
    }
}
