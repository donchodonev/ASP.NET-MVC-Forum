namespace ASP.NET_MVC_Forum.Data
{
    public class DataConstants
    {
        public class UserConstants
        {
            public const int FirstNameMaxLength = 15;

            public const int LastNameMaxLength = 15;

            public const int FirstNameMinLength = 2;

            public const int LastNameMinLength = 2;

            public const int AgeFloor = 0;

            public const int AgeCeiling = 120;

            public const int UsernameMaxLength = 20;

            public const int UsernameMinLength = 4;
        }

        public class CategoryConstants
        {
            public const int NameMaxLength = 100;
        }

        public class PostConstants
        {
            public const int NameMaxLength = 100;

            public const int NameMinLength = 10;

            public const int HtmlContentMaxLength = 5000;

            public const int HtmlContentMinLength = 100;
        }

        public class CommentConstants
        {
            public const int ContentMaxLength = 500;

            public const int ContentMinLength = 1;
        }

        public class RoleConstants
        {
            public const string AdminRoleName = "Administrator";

            public const string ModeratorRoleName = "Moderator";
        }
    }
}
