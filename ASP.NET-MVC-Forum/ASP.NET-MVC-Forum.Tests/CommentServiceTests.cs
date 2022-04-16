namespace ASP.NET_MVC_Forum.Tests
{
    using ASP.NET_MVC_Forum.Business;
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Models.Comment;
    using ASP.NET_MVC_Forum.Infrastructure.MappingProfiles;
    using ASP.NET_MVC_Forum.Validation.Contracts;

    using AutoMapper;

    using Microsoft.EntityFrameworkCore;

    using Moq;

    using NUnit.Framework;

    using System;
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
        public async Task SetUpAsync()
        {
            mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(new CommentMappingProfile()));
            mapper = new Mapper(mapperConfiguration);
            dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("ForumDb").Options;
            dbContext = new ApplicationDbContext(dbContextOptions);
            commentRepository = new CommentRepository(mapper, dbContext);
            postValidationServiceMock = new Mock<IPostValidationService>();
            commentReportServiceMock = new Mock<ICommentReportService>();
            commentValidationServiceMock = new Mock<ICommentValidationService>();
            commentService = new CommentService(
                commentRepository,
                mapper,
                postValidationServiceMock.Object,
                commentReportServiceMock.Object,
                commentValidationServiceMock.Object);
            await SeedTestData();
        }

        [Test]
        public async Task Test()
        {
        }

        private async Task SeedTestData()
        {
            string userId = "some id";

            string commentText = "Test text";

            int postId = 1;

            await SeedUser(userId);
            await SeedPost(postId);
            await SeedComment(userId, commentText, postId);
        }

        private Task SeedComment(string userId, string commentText, int postId)
        {
            var model = new CommentPostRequestModel() { CommentText = commentText, PostId = postId };

            return commentRepository.AddCommentAsync(model, userId);
        }

        private Task SeedUser(string userId)
        {
            dbContext.Users.Add(new ExtendedIdentityUser() { Id = userId });

            return dbContext.SaveChangesAsync();
        }

        private Task SeedPost(int postId)
        {
            dbContext.Posts.Add(new Post() { Id = postId });
            return dbContext.SaveChangesAsync();
        }
    }
}
