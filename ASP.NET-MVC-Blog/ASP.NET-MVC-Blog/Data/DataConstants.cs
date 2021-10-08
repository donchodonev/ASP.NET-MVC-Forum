namespace ASP.NET_MVC_Blog.Data
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
    }
}
