namespace ASP.NET_MVC_Forum.Data.Constants
{
    public class RoleConstants
    {
        public const string AdminRoleName = "Administrator";

        public const string ModeratorRoleName = "Moderator";

        public const string AdminOrModerator = AdminRoleName + "," + ModeratorRoleName;
    }
}
