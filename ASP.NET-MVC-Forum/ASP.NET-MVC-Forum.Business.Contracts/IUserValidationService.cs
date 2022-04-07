namespace ASP.NET_MVC_Forum.Business.Contracts.Contracts
{
    using ASP.NET_MVC_Forum.Domain.Entities;

    using System.Security.Claims;
    using System.Threading.Tasks;

    public interface IUserValidationService
    {
        public Task ValidateUserIsPrivilegedAsync(int postId, ClaimsPrincipal user);

        public void ValidateUsername(string username);

        public void ValidateNotNull(ExtendedIdentityUser user);

        public Task ValidateUserExistsAsync(string username);
    }
}
