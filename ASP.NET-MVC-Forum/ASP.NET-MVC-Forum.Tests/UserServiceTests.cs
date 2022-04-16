namespace ASP.NET_MVC_Forum.Tests
{
    using ASP.NET_MVC_Forum.Business;
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Domain.Exceptions;
    using ASP.NET_MVC_Forum.Domain.Models.User;
    using ASP.NET_MVC_Forum.Infrastructure.MappingProfiles;
    using ASP.NET_MVC_Forum.Validation.Contracts;

    using AutoMapper;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using Moq;

    using NUnit.Framework;

    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class UserServiceTests
    {
        private Mock<IUserStore<ExtendedIdentityUser>> store;

        private MapperConfiguration mapperConfiguration;

        private IMapper mapper;

        private UserMappingProfile userMapperProfile;

        private DbContextOptions<ApplicationDbContext> dbContextOptions;

        private ApplicationDbContext dbContext;

        private IUserService userService;

        private IUserRepository userRepo;

        private Mock<UserManager<ExtendedIdentityUser>> userManagerMock;

        private IAvatarRepository avatarRepo;

        private Mock<IAvatarRepository> avatarRepoMock;

        private Mock<IWebHostEnvironment> webHostEnvironmentMock;

        private Mock<IUserValidationService> userValidationServiceMock;

        private Mock<IFormFile> formFileMock;

        private const string USERNAME = "some USERNAME";

        private const string FIRSTNAME = "some FIRSTNAME";

        private const string LASTNAME = "some LASTNAME";

        private const string ID = "some id";

        private const string EMAIL = "email@email.email";

        private ExtendedIdentityUser DEFAULT_USER = new ExtendedIdentityUser()
        {
            Id = ID,
            UserName = USERNAME,
            FirstName = FIRSTNAME,
            LastName = LASTNAME,
            Email = EMAIL,
            ImageUrl = ""
        };

        [SetUp]
        public void SetUp()
        {
            store = new Mock<IUserStore<ExtendedIdentityUser>>();

            avatarRepoMock = new Mock<IAvatarRepository>();

            userMapperProfile = new UserMappingProfile();

            mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile(userMapperProfile));

            mapper = new Mapper(mapperConfiguration);

            dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("ForumDb7")
                .Options;

            dbContext = new ApplicationDbContext(dbContextOptions);

            webHostEnvironmentMock = new Mock<IWebHostEnvironment>();

            avatarRepoMock = new Mock<IAvatarRepository>();

            avatarRepo = new AvatarRepository(webHostEnvironmentMock.Object);

            userManagerMock = new Mock<UserManager<ExtendedIdentityUser>>(store.Object, null, null, null, null, null, null, null, null);

            userManagerMock.Object.UserValidators.Add(new UserValidator<ExtendedIdentityUser>());

            userManagerMock.Object.PasswordValidators.Add(new PasswordValidator<ExtendedIdentityUser>());

            userValidationServiceMock = new Mock<IUserValidationService>();

            userRepo = new UserRepository(dbContext, userManagerMock.Object, avatarRepoMock.Object);

            formFileMock = new Mock<IFormFile>();

            userService = new UserService(userRepo,
                userValidationServiceMock.Object,
                mapper,
                userManagerMock.Object);
        }

        [TearDown]
        public async Task TeardownAsync()
        {
            var users = await dbContext.Users.ToListAsync();

            dbContext.Users.RemoveRange(users);

            await dbContext.SaveChangesAsync();
        }

        [Test]
        public void AvatarUpdateAsync_ShouldThrowException_WhenUserNotFoundById()
        {
            MockValidateUserNotNullMethod();

            Assert.ThrowsAsync<NullUserException>(() =>
            userService.AvatarUpdateAsync(It.IsAny<string>(), It.IsAny<IFormFile>()));
        }

        [Test]
        public async Task AvatarUpdateAsync_ShouldUpdateUserImageURL()
        {
            avatarRepoMock
                .Setup(x => x.UploadAvatarAsync(
                    It.IsAny<IFormFile>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync("some url");

            MockUserManagerFindByIdMethod();

            await SeedUserAsync();

            var user = await userRepo
                .GetAll()
                .FirstOrDefaultAsync();

            var imageSizeInBytes = 5242879;

            string imageName = "name.";

            string imageExtension = "JPG";

            formFileMock.Setup(x => x.Length).Returns(imageSizeInBytes);

            formFileMock.Setup(x => x.FileName).Returns(imageName + imageExtension);

            webHostEnvironmentMock.Setup(x => x.ContentRootPath)
                .Returns("C:\\Users\\donev\\source\\repos\\ASP.NET-MVC-Forum\\ASP.NET-MVC-Forum\\ASP.NET-MVC-Forum");

            Assert.IsTrue(user.ImageUrl == "");

            await userService.AvatarUpdateAsync(user.Id, formFileMock.Object);

            Assert.IsFalse(user.ImageUrl == "");
        }

        [Test]
        public async Task GenerateUserVoidModelAsync_ShouldReturnSameCountOfUsersAsUsersInDbTransformed_As_UserViewModel()
        {
            await SeedUserAsync();

            MockUserManagerGetRolesMethod();

            var model = await userService.GenerateUserViewModelAsync();

            Assert.NotNull(model);
            Assert.IsAssignableFrom<List<UserViewModel>>(model);
            Assert.IsTrue(model.Count == await userRepo.GetAll().CountAsync());
        }

        private void MockValidateUserNotNullMethod()
        {
            userValidationServiceMock
                .Setup(x => x.ValidateUserNotNull(It.IsAny<ExtendedIdentityUser>()))
                .Throws<NullUserException>();
        }

        private void MockUserManagerFindByIdMethod()
        {
            userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(DEFAULT_USER);
        }

        private void MockUserManagerGetRolesMethod()
        {
            userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ExtendedIdentityUser>()))
                .ReturnsAsync(new List<string>() { "Some role"});
        }

        private Task SeedUserAsync()
        {
            dbContext
                .Users
                .Add(DEFAULT_USER);

            return dbContext.SaveChangesAsync();
        }
    }
}
