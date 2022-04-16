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

    using AutoMapper;

    using Microsoft.EntityFrameworkCore;

    using Moq;

    using NUnit.Framework;

    using System.Collections.Generic;
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

        string userId;
        string username;
        string commentText;
        int postId;
        int commentId = 1;
        bool isInAdminOrModeratorRole;

        [SetUp]
        public async Task SetUpAsync()
        {
            mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(new CommentMappingProfile()));

            mapper = new Mapper(mapperConfiguration);

            dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("ForumDb")
                .Options;

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

            userId = "some id";

            username = "some username";

            commentText = "Test text";

            postId = 1;

            commentId = 1;

            isInAdminOrModeratorRole = true;

            await SeedTestDataAsync();
        }


        [Test]
        public void GenerateCommentGetResponseModel_ShouldThrowException_When_PostDoes_NOT_Exist()
        {
            postValidationServiceMock
                .Setup(x => x.ValidatePostExistsAsync(postId))
                .ThrowsAsync(new EntityDoesNotExistException());

            Assert.ThrowsAsync<EntityDoesNotExistException>(() => commentService.GenerateCommentGetResponseModelAsync(postId));
        }

        [Test]
        public async Task GenerateCommentGetRequestResponseModel_ShouldReturn_ListOf_CommentGetRequestResponseModel()
        {
            postValidationServiceMock
                .Setup(x => x.ValidateNotNull(postId));

            int expectedCountOfModelsReturned = 1;

            List<CommentGetRequestResponseModel> models = await commentService.GenerateCommentGetResponseModelAsync(postId);

            var actualCountOfModelsReturn = models.Count;

            Assert.AreEqual(expectedCountOfModelsReturned, actualCountOfModelsReturn);

            Assert.NotNull(models);
        }

        [Test]
        public async Task GenerateCommentPostResponseModelAsync_ShouldCreateCommentPostRequestResponseModel()
        {
            var commentData = new CommentPostRequestModel() { CommentText = commentText, PostId = postId };

            var model = await commentService.GenerateCommentPostResponseModelAsync(commentData, userId, username, commentId);

            Assert.NotNull(model);
            Assert.AreEqual(model.CommentText, commentText);
            Assert.AreEqual(model.Username, username);
            Assert.AreEqual(model.Id, commentId);
            Assert.AreEqual(model.Username, username);
            Assert.AreEqual(model.UserId, userId);
            Assert.AreEqual(model.PostId, postId);
        }

        [Test]
        public async Task DeleteAsync_ShouldThrowException_When_Comment_IsNotFound_ById()
        {
            await TeardownAsync();

            commentValidationServiceMock
                .Setup(x => x.ValidateCommentNotNull(null))
                .Throws<NullCommentException>();

            Assert.ThrowsAsync<NullCommentException>(() => 
            commentService.DeleteAsync(commentId, userId, isInAdminOrModeratorRole));
        }

        [Test]
        public void DeleteAsync_ShouldThrowException_When_User_NOT_AllowedToDelete()
        {
            commentValidationServiceMock
                .Setup(x => x.ValidateUserCanDeleteCommentAsync(commentId,userId,isInAdminOrModeratorRole))
                .Throws<InsufficientPrivilegeException>();

            Assert.ThrowsAsync<InsufficientPrivilegeException>(() =>
            commentService.DeleteAsync(commentId, userId, isInAdminOrModeratorRole));
        }

        [Test]
        public async Task DeleteAsync_ShouldMarkCommentAsDeleted_When_CommentExists_And_User_HasPrivilegeToDelete ()
        {
            commentValidationServiceMock
                .Setup(x => x.ValidateUserCanDeleteCommentAsync(commentId, userId, isInAdminOrModeratorRole))
                .Returns(Task.CompletedTask);

            bool commentExists = await commentRepository.ExistsAsync(commentId);

            Assert.True(commentExists);

            await commentService.DeleteAsync(commentId, userId, isInAdminOrModeratorRole);

            commentExists = await commentRepository.ExistsAsync(commentId);

            Assert.False(commentExists);
        }

        private async Task SeedTestDataAsync()
        {
            await TeardownAsync();

            await SeedUser();
            await SeedPost();
            await SeedComment();
        }

        private Task SeedComment()
        {
            var comment = new Comment() 
            { 
                Id = commentId,
                UserId = userId,
                Content = commentText,
                PostId = postId 
            };

            dbContext.Comments.Add(comment);

            return dbContext.SaveChangesAsync();
        }

        private Task SeedUser()
        {
            dbContext.Users.Add(new ExtendedIdentityUser() { Id = userId, UserName = username });

            return dbContext.SaveChangesAsync();
        }

        private Task SeedPost()
        {
            dbContext.Posts.Add(new Post() { Id = postId });
            return dbContext.SaveChangesAsync();
        }

        private async Task TeardownAsync()
        {
            var posts = await dbContext.Posts.ToListAsync();
            var users = await dbContext.Users.ToListAsync();
            var comments = await dbContext.Comments.ToListAsync();

            dbContext.Posts.RemoveRange(posts);
            dbContext.Users.RemoveRange(users);
            dbContext.Comments.RemoveRange(comments);

            await dbContext.SaveChangesAsync();
        }
    }
}
