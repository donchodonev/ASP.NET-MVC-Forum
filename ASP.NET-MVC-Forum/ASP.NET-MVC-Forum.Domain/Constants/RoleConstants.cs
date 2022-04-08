namespace ASP.NET_MVC_Forum.Domain.Constants
{
    public class RoleConstants
    {
        public const string ADMIN_ROLE = "Administrator";

        public const string MODERATOR_ROLE = "Moderator";

        public const string ADMIN_OR_MODERATOR = ADMIN_ROLE + "," + MODERATOR_ROLE;
    }
}
