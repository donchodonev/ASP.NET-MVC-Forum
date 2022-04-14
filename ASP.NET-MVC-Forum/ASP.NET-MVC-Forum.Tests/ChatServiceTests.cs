namespace ASP.NET_MVC_Forum.Tests
{
    using ASP.NET_MVC_Forum.Business;
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Exceptions;
    using ASP.NET_MVC_Forum.Infrastructure;
    using ASP.NET_MVC_Forum.Infrastructure.MappingProfiles;
    using ASP.NET_MVC_Forum.Validation.Contracts;

    using AutoMapper;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using Moq;

    using NUnit.Framework;

    using System.Linq;
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
        private IChatRepository chatRepository;
        private Mock<IUserValidationService> userValidationServiceMock;
        private Mock<IChatValidationService> chatValidationServiceMock;
        private Mock<IUserStore<ExtendedIdentityUser>> store;
        private Mock<IAvatarRepository> avatarRepositoryMock;
        private UserManager<ExtendedIdentityUser> userManager;
        private Mock<IHtmlManipulator> htmlManipulatorMock;

        private string recipientUsername;
        private string senderUsername;
        private string senderId;
        private string recipientId;

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
            htmlManipulatorMock = new Mock<IHtmlManipulator>();
            chatRepository = new ChatRepository(dbContext, htmlManipulatorMock.Object);
            chatService = new ChatService(mapper, userRepository, chatRepository, userValidationServiceMock.Object, chatValidationServiceMock.Object);

            recipientUsername = "recipient username";
            recipientId = "recipient id";
            senderUsername = "sender username";
            senderId = "sender id";
        }

        [TearDown]
        public async Task TeardownAsync()
        {
            var users = dbContext.Users;
            var chats = dbContext.Chats;

            dbContext.Users.RemoveRange(users);
            dbContext.Chats.RemoveRange(chats);

            await dbContext.SaveChangesAsync();
        }

        [Test]
        public void GenerateChatSelectUserViewModel_ShouldThrowException_WhenSenderOrRecipientDoesNotExist()
        {
            userValidationServiceMock
                .Setup(x => x.ValidateUserExistsByUsernameAsync(It.IsAny<string>()))
                .ThrowsAsync(new NullUserException());

            Assert.ThrowsAsync<NullUserException>(() => chatService.GenerateChatSelectUserViewModel(recipientUsername, senderId, senderUsername));
        }

        [Test]
        public async Task GenerateChatSelectUserViewModel_ShouldReturnChatSelectUserViewModel_WhenSenderAndRecipientExist()
        {
            string imageUrl = "test url";

            var dummyUser = new ExtendedIdentityUser() { UserName = recipientUsername, ImageUrl = imageUrl };

            await AddUserAsync(dummyUser);

            userValidationServiceMock
                .Setup(x => x.ValidateUserExistsByUsernameAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            var result = await chatService.GenerateChatSelectUserViewModel(recipientUsername, senderId, senderUsername);

            Assert.NotNull(result);
            Assert.AreEqual(result.SenderUsername, senderUsername);
            Assert.AreEqual(result.RecipientUsername, recipientUsername);
            Assert.AreEqual(result.RecipientIdentityUserId, dummyUser.Id);
            Assert.AreEqual(result.SenderIdentityUserId, senderId);
            Assert.AreEqual(result.ImageUrl, imageUrl);
        }

        [Test]
        public void GenerateChatConversationViewModel_ShouldThrowException_WhenSenderOrRecipientDoesNotExist()
        {
            userValidationServiceMock
                .Setup(x => x.ValidateUserExistsByIdAsync(It.IsAny<string>()))
                .ThrowsAsync(new NullUserException());

            Assert.ThrowsAsync<NullUserException>(() => chatService.GenerateChatConversationViewModel(senderId, recipientId, senderUsername));
        }

        [Test]
        public async Task GenerateChatConversationViewModel_ShouldReturnChatConversationViewModel_WhenSenderAndRecipientExist()
        {
            var dummyUser = new ExtendedIdentityUser() { UserName = recipientUsername };

            await AddUserAsync(dummyUser);

            userValidationServiceMock
                .Setup(x => x.ValidateUserExistsByUsernameAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            var result = await chatService.GenerateChatConversationViewModel(senderId, recipientId, senderUsername);

            var chatId = dbContext.Chats.Select(x => x.Id).First();

            Assert.NotNull(result);
            Assert.AreEqual(result.SenderUsername, senderUsername);
            Assert.AreEqual(result.ReceiverIdentityUserId, recipientId);
            Assert.AreEqual(result.SenderIdentityUserId, senderId);
            Assert.AreEqual(result.ChatId, chatId);
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
