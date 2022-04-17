namespace ASP.NET_MVC_Forum.Tests.Fakes
{
    using ASP.NET_MVC_Forum.Domain.Entities;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using Moq;

    using System.Security.Claims;
    using System.Threading.Tasks;

    public class FakeSignInManager : SignInManager<ExtendedIdentityUser>
    {
        #region Fields
        private readonly bool _simulateSuccess = false;
        #endregion

        #region Constructors
        public FakeSignInManager(bool simulateSuccess = true)
              : base(new Mock<FakeUserManager>().Object,
                     new Mock<IHttpContextAccessor>().Object,
                     new Mock<IUserClaimsPrincipalFactory<ExtendedIdentityUser>>().Object,
                     new Mock<IOptions<IdentityOptions>>().Object,
                     new Mock<ILogger<SignInManager<ExtendedIdentityUser>>>().Object,
                     new Mock<IAuthenticationSchemeProvider>().Object,
                     new Mock<IUserConfirmation<ExtendedIdentityUser>>().Object)
        {
            this._simulateSuccess = simulateSuccess;
        }
        #endregion

        #region Public methods
        public override Task<SignInResult> PasswordSignInAsync(ExtendedIdentityUser user, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return this.ReturnResult(this._simulateSuccess);
        }

        public override Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return this.ReturnResult(this._simulateSuccess);
        }

        public override Task<SignInResult> CheckPasswordSignInAsync(ExtendedIdentityUser user, string password, bool lockoutOnFailure)
        {
            return this.ReturnResult(this._simulateSuccess);
        }

        public override bool IsSignedIn(ClaimsPrincipal principal)
        {
            return _simulateSuccess;
        }
        #endregion

        #region Internal methods
        private Task<SignInResult> ReturnResult(bool isSuccess = true)
        {
            SignInResult result = SignInResult.Success;

            if (!isSuccess)
                result = SignInResult.Failed;

            return Task.FromResult(result);
        }
        #endregion
    }
}