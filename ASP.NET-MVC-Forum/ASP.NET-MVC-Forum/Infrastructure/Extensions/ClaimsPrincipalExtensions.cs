namespace ASP.NET_MVC_Forum.Infrastructure.Extensions
{
    using System.Security.Claims;

    using static ASP.NET_MVC_Forum.Data.DataConstants.RoleConstants;
    public static class ClaimsPrincipalExtensions
    {
        public static string Id(this ClaimsPrincipal user)
            => user.FindFirst(ClaimTypes.NameIdentifier).Value;

        public static bool IsAdmin(this ClaimsPrincipal user)
            => user.IsInRole(AdminRoleName);
    }
}
