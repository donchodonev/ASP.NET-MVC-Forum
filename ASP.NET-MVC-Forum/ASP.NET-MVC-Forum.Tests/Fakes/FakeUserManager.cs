namespace ASP.NET_MVC_Forum.Tests.Fakes
{
    using ASP.NET_MVC_Forum.Domain.Entities;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using Moq;

    using System;
    using System.Threading.Tasks;

    public class FakeUserManager : UserManager<ExtendedIdentityUser>
    {
        public FakeUserManager()
            : base(new Mock<IUserStore<ExtendedIdentityUser>>().Object,
              new Mock<IOptions<IdentityOptions>>().Object,
              new Mock<IPasswordHasher<ExtendedIdentityUser>>().Object,
              new IUserValidator<ExtendedIdentityUser>[0],
              new IPasswordValidator<ExtendedIdentityUser>[0],
              new Mock<ILookupNormalizer>().Object,
              new Mock<IdentityErrorDescriber>().Object,
              new Mock<IServiceProvider>().Object,
              new Mock<ILogger<UserManager<ExtendedIdentityUser>>>().Object)
        { }

        public override Task<IdentityResult> CreateAsync(ExtendedIdentityUser user, string password)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<IdentityResult> AddToRoleAsync(ExtendedIdentityUser user, string role)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<string> GenerateEmailConfirmationTokenAsync(ExtendedIdentityUser user)
        {
            return Task.FromResult(Guid.NewGuid().ToString());
        }

        public override Task<IdentityResult> ConfirmEmailAsync(ExtendedIdentityUser user, string token)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<ExtendedIdentityUser> FindByEmailAsync(string email)
        {
            return base.FindByEmailAsync(email);
        }
    }
}


