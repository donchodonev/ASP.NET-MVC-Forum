namespace ASP.NET_MVC_Forum.Tests
{
    using ASP.NET_MVC_Forum.Business;
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Exceptions;
    using ASP.NET_MVC_Forum.Domain.Models.Post;
    using ASP.NET_MVC_Forum.Domain.Models.Votes;
    using ASP.NET_MVC_Forum.Infrastructure.MappingProfiles;
    using ASP.NET_MVC_Forum.Validation.Contracts;

    using AutoMapper;

    using Microsoft.EntityFrameworkCore;

    using Moq;

    using NUnit.Framework;

    using System.Threading.Tasks;

    public class VoteServiceTests
    {
        private MapperConfiguration mapperConfiguration;

        private IMapper mapper;

        private VoteMappingProfile voteMapperProfile;

        private DbContextOptions<ApplicationDbContext> dbContextOptions;

        private ApplicationDbContext dbContext;

        private IVoteRepository voteRepo;

        private IVoteService voteService;

        private Mock<IUserValidationService> userValidationServiceMock;

        private Mock<IPostValidationService> postValidationServiceMock;

        private const string USER_ID = "some user id";

        private const int POST_ID = 1;

        [SetUp]
        public void SetUp()
        {
            voteMapperProfile = new VoteMappingProfile();

            mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(voteMapperProfile));

            mapper = new Mapper(mapperConfiguration);

            dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("ForumDb8")
                .Options;

            dbContext = new ApplicationDbContext(dbContextOptions);

            voteRepo = new VoteRepository(dbContext);

            userValidationServiceMock = new Mock<IUserValidationService>();

            postValidationServiceMock = new Mock<IPostValidationService>();

            voteService = new VoteService(voteRepo,
                mapper,
                userValidationServiceMock.Object,
                postValidationServiceMock.Object);
        }

        [TearDown]
        public async Task TeardownAsync()
        {
            var users = await dbContext.Users.ToListAsync();
            var posts = await dbContext.Posts.ToListAsync();

            dbContext.Users.RemoveRange(users);
            dbContext.Posts.RemoveRange(posts);

            await dbContext.SaveChangesAsync();
        }

        [Test]
        public void RegisterVoteAsync_ShouldThrowError_WhenUserNotFound_ById()
        {
            UserExistsByIdMethodMock();

            Assert.ThrowsAsync<NullUserException>(() =>
            voteService.RegisterVoteAsync(It.IsAny<VoteRequestModel>(), It.IsAny<string>()));
        }

        [Test]
        public async Task RegisterVoteAsync_ShouldRegister_PositiveVote()
        {
            await SeedUserAsync();

            await SeedPostAsync();

            var vote = new VoteRequestModel() { PostId = POST_ID, IsPositiveVote = true };

            bool isPostUpvoted = await voteService.GetPostVoteSumAsync(POST_ID) > 0;

            Assert.IsFalse(isPostUpvoted);

            await voteService.RegisterVoteAsync(vote, USER_ID);

            isPostUpvoted = await voteService.GetPostVoteSumAsync(POST_ID) > 0;

            Assert.IsTrue(isPostUpvoted);
        }

        [Test]
        public async Task RegisterVoteAsync_ShouldRegister_NegativeVote()
        {
            await SeedUserAsync();
            await SeedPostAsync();

            var vote = new VoteRequestModel() { PostId = POST_ID, IsPositiveVote = false };

            bool isPostDownvoted = await voteService.GetPostVoteSumAsync(POST_ID) < 0;

            Assert.IsFalse(isPostDownvoted);

            await voteService.RegisterVoteAsync(vote, USER_ID);

            isPostDownvoted = await voteService.GetPostVoteSumAsync(POST_ID) < 0;

            Assert.IsTrue(isPostDownvoted);
        }

        [Test]
        public void GetPostVoteSumAsync_ShouldThrowException_WhenPostIsNotFound_ById()
        {
            postValidationServiceMock
                .Setup(x => x.ValidatePostExistsAsync(It.IsAny<int>()))
                .Throws<EntityDoesNotExistException>();

            Assert.ThrowsAsync<EntityDoesNotExistException>(() => voteService.GetPostVoteSumAsync(It.IsAny<int>()));
        }

        [Test]
        public async Task GetPostVoteSumAsync_Should_GetPostVoteSum()
        {
            var negativeVote = new VoteRequestModel() { PostId = POST_ID, IsPositiveVote = false };

            var positiveVote = new VoteRequestModel() { PostId = POST_ID, IsPositiveVote = true };

            await SeedUserAsync();

            await SeedPostAsync();

            Assert.IsTrue(await voteService.GetPostVoteSumAsync(POST_ID) == 0);

            await voteService.RegisterVoteAsync(negativeVote, USER_ID);

            Assert.IsTrue(await voteService.GetPostVoteSumAsync(POST_ID) == -1);

            await voteService.RegisterVoteAsync(positiveVote, USER_ID);
            await voteService.RegisterVoteAsync(positiveVote, USER_ID);

            Assert.IsTrue(await voteService.GetPostVoteSumAsync(POST_ID) == 1);
        }

        [Test]
        public void InjectUserLastVoteType_ShouldThrowException_WhenUserIsNotFound_ById()
        {
            UserExistsByIdMethodMock();

            Assert.ThrowsAsync<NullUserException>(() =>
            voteService.InjectUserLastVoteType(It.IsAny<ViewPostViewModel>(), It.IsAny<string>()));
        }

        [Test]
        public async Task InjectUserLastVoteType_ShouldUpdateViewPostViewModel_UserLastVoteChoiceProperty()
        {
            await SeedUserAsync();

            await SeedPostAsync();

            var model = new ViewPostViewModel() { PostId = POST_ID };

            int initialVoteChoice = model.UserLastVoteChoice;

            var positiveVote = new VoteRequestModel() { PostId = POST_ID, IsPositiveVote = true };

            await voteService.RegisterVoteAsync(positiveVote, USER_ID);

            await voteService.InjectUserLastVoteType(model, USER_ID);

            int voteChoiceAfterInjection = model.UserLastVoteChoice;

            Assert.AreNotEqual(initialVoteChoice, voteChoiceAfterInjection);
        }

        private void UserExistsByIdMethodMock()
        {
            userValidationServiceMock
                .Setup(x => x.ValidateUserExistsByIdAsync(It.IsAny<string>()))
                .Throws<NullUserException>();
        }

        private Task SeedUserAsync()
        {
            var user = new ExtendedIdentityUser() { Id = USER_ID };

            dbContext.Users.Add(user);

            return dbContext.SaveChangesAsync();
        }

        private Task SeedPostAsync()
        {
            var post = new Post() { Id = POST_ID };

            dbContext.Posts.Add(post);

            return dbContext.SaveChangesAsync();
        }
    }
}
