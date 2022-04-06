namespace ASP.NET_MVC_Forum.Business.Contracts.Contracts
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    public interface IUserValidationService
    {
        public Task ValidateUserIsPrivilegedAsync(int postId, ClaimsPrincipal user);
    }
}
