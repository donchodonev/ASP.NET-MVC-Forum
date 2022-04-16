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

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.RoleConstants;

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

        private Mock<IAvatarRepository> avatarRepoMock;

        private Mock<IWebHostEnvironment> webHostEnvironmentMock;

        private Mock<IUserValidationService> userValidationServiceMock;

        private Mock<IFormFile> formFileMock;

        private const string USERNAME = "some USERNAME";

        private const string FIRSTNAME = "some FIRSTNAME";

        private const string LASTNAME = "some LASTNAME";

        private const string USER_ID = "some id";

        private const string EMAIL = "email@email.email";

        private const string ROLE_ID = "some role id";

        private ExtendedIdentityUser DEFAULT_USER = new ExtendedIdentityUser()
        {
            Id = USER_ID,
            UserName = USERNAME,
            FirstName = FIRSTNAME,
            LastName = LASTNAME,
            Email = EMAIL,
            ImageUrl = "",
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

        [Test]
        public void BanAsync_ShouldThrowException_WhenUserNotFoundById()
        {
            MockValidateUserNotNullMethod();

            Assert.ThrowsAsync<NullUserException>(() => userService.BanAsync(It.IsAny<string>()));
        }

        [Test]
        public void BanAsync_ShouldThrowException_WhenUserIsBannedAlready()
        {
            userValidationServiceMock
                .Setup(x => x.ValidateUserIsNotBannedAsync(It.IsAny<string>()))
                .Throws<UserIsBannedException>();

            Assert.ThrowsAsync<UserIsBannedException>(() => userService.BanAsync(It.IsAny<string>()));
        }

        [Test]
        public async Task BanAsync_ShouldBanUser()
        {
            MockUserManagerFindByIdMethod();
            MockUserManagerUpdateSecurityStampMethod();

            var user = await userRepo.GetByIdAsync(USER_ID);

            Assert.False(user.IsBanned);
            Assert.False(user.LockoutEnabled);

            await userService.BanAsync(USER_ID);

            Assert.True(user.IsBanned);
            Assert.True(user.LockoutEnabled);
        }

        [Test]
        public void UnbanAsync_ShouldThrowException_WhenUserNotFoundById()
        {
            MockValidateUserNotNullMethod();

            Assert.ThrowsAsync<NullUserException>(() => userService.UnbanAsync(It.IsAny<string>()));
        }

        [Test]
        public void UnbanAsync_ShouldThrowException_IsNotBanned()
        {
            userValidationServiceMock
                .Setup(x => x.ValidateUserIsBannedAsync(It.IsAny<string>()))
                .Throws<UserIsBannedException>();


            Assert.ThrowsAsync<UserIsBannedException>(() => userService.UnbanAsync(It.IsAny<string>()));
        }

        [Test]
        public async Task UnbanAsync_ShouldUnbanUser()
        {
            MockUserManagerFindByIdMethod();
            MockUserManagerUpdateSecurityStampMethod();

            await userService.BanAsync(USER_ID);

            var user = await userRepo.GetByIdAsync(USER_ID);

            Assert.True(user.IsBanned);
            Assert.True(user.LockoutEnabled);

            await userService.UnbanAsync(USER_ID);

            Assert.False(user.IsBanned);
            Assert.False(user.LockoutEnabled);
        }

        [Test]
        public void DemoteAsync_ShouldThrowException_WhenUserIsNotFoundbyId()
        {
            userValidationServiceMock.
                Setup(x => x.ValidateUserExistsByIdAsync(It.IsAny<string>()))
                .Throws<NullUserException>();

            Assert.ThrowsAsync<NullUserException>(() => userService.DemoteAsync(It.IsAny<string>()));
        }

        [Test]
        public void DemoteAsync_ShouldThrowException_WhenUserIsNotModerator()
        {
            userValidationServiceMock.
                Setup(x => x.ValidateUserIsModerator(It.IsAny<string>()))
                .Throws<InvalidRoleException>();

            Assert.ThrowsAsync<InvalidRoleException>(() => userService.DemoteAsync(It.IsAny<string>()));
        }

        [Test]
        public void PromoteAsync_ShouldThrowException_WhenUserIsNotFoundbyId()
        {
            MockValidateUserNotNullMethod();

            Assert.ThrowsAsync<NullUserException>(() => userService.PromoteAsync(It.IsAny<string>()));
        }


        [Test]
        public void PromoteAsync_ShouldThrowException_WhenUserIsModerator()
        {
            userValidationServiceMock.
                Setup(x => x.ValidateUserIsNotModerator(It.IsAny<ExtendedIdentityUser>()))
                .Throws<InvalidRoleException>();

            Assert.ThrowsAsync<InvalidRoleException>(() => userService.PromoteAsync(It.IsAny<string>()));
        }

        private void MockValidateUserNotNullMethod()
        {
            userValidationServiceMock
                .Setup(x => x.ValidateUserNotNull(It.IsAny<ExtendedIdentityUser>()))
                .Throws<NullUserException>();
        }

        private void MockUserManagerUpdateSecurityStampMethod()
        {
            userManagerMock
                .Setup(x => x.UpdateSecurityStampAsync(It.IsAny<ExtendedIdentityUser>()))
                .ReturnsAsync(new IdentityResult());
        }

        private void MockUserManagerFindByIdMethod()
        {
            userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(DEFAULT_USER);
        }

        private void MockUserManagerGetRolesMethod()
        {
            userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ExtendedIdentityUser>()))
                .ReturnsAsync(new List<string>() { "Some role" });
        }

        private Task SeedUserAsync()
        {
            dbContext
                .Users
                .Add(DEFAULT_USER);

            return dbContext.SaveChangesAsync();
        }

        private Task SeedUserRoleAsync()
        {
            dbContext.Roles.Add(new IdentityRole()
            {
                Id = ROLE_ID,
                Name = MODERATOR_ROLE,
                NormalizedName = MODERATOR_ROLE.ToUpper()
            });

            return dbContext.SaveChangesAsync();
        }

        private Task AddUserToModeratorRoleAsync()
        {
            dbContext.UserRoles.Add(new IdentityUserRole<string>() { RoleId = ROLE_ID, UserId = USER_ID });

            return dbContext.SaveChangesAsync();
        }
    }
}
