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

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using Moq;

    using NUnit.Framework;

    using System.Threading.Tasks;

    internal class ChatServiceTests
    {
        private DbContextOptions<ApplicationDbContext> dbContextOptions;
        private ApplicationDbContext dbContext;
        private MapperConfiguration mapperConfiguration;
        private IMapper mapper;
        private IChatService chatService;
        private UserMappingProfile userMappingProfile;
        private IUserRepository userRepository;
        private Mock<IChatRepository> chatRepository;
        private Mock<IUserValidationService> userValidationServiceMock;
        private Mock<IChatValidationService> chatValidationServiceMock;
        private Mock<IUserStore<ExtendedIdentityUser>> store;
        private Mock<IAvatarRepository> avatarRepositoryMock;
        private UserManager<ExtendedIdentityUser> userManager;


        [SetUp]
        public void SetUp()
        {
            dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("ForumDb").Options;
            dbContext = new ApplicationDbContext(dbContextOptions);
            userValidationServiceMock = new Mock<IUserValidationService>();
            chatValidationServiceMock = new Mock<IChatValidationService>();
            userMappingProfile = new UserMappingProfile();
            mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(userMappingProfile));
            mapper = new Mapper(mapperConfiguration);
            store = new Mock<IUserStore<ExtendedIdentityUser>>();
            avatarRepositoryMock = new Mock<IAvatarRepository>();
            userManager = new UserManager<ExtendedIdentityUser>(store.Object, null, null, null, null, null, null, null, null);
            userRepository = new UserRepository(dbContext, userManager, avatarRepositoryMock.Object);
            chatRepository = new Mock<IChatRepository>();
            chatService = new ChatService(mapper, userRepository, chatRepository.Object, userValidationServiceMock.Object, chatValidationServiceMock.Object);
        }

        [Test]
        public void GenerateChatSelectUserViewModel_ShouldThrowException_WhenSenderOrRecipientDoesNotExist()
        {
            userValidationServiceMock
                .Setup(x => x.ValidateUserExistsByUsernameAsync(It.IsAny<string>()))
                .ThrowsAsync(new NullUserException());

            Assert.ThrowsAsync<NullUserException>(() => chatService.GenerateChatSelectUserViewModel("recipient username", "sender id", "sender username"));
        }

        [Test]
        public async Task GenerateChatSelectUserViewModel_ShouldReturnChatSelectUserViewModel_WhenSenderAndRecipientExist()
        {
            var recipientUsername = "recipient username";
            var senderUsername = "sender username";
            var senderId = "sender id";
            var dummyUser = new ExtendedIdentityUser() { UserName = recipientUsername };

            await AddUserAsync(dummyUser);

            userValidationServiceMock
                .Setup(x => x.ValidateUserExistsByUsernameAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            var result = await chatService.GenerateChatSelectUserViewModel(recipientUsername, senderId, senderUsername);

            Assert.AreEqual(result.SenderUsername, senderUsername);
            Assert.AreEqual(result.RecipientUsername, recipientUsername);
        }

        private async Task AddUserAsync(ExtendedIdentityUser user)
        {
            if (!await dbContext.Users.AnyAsync())
            {
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
