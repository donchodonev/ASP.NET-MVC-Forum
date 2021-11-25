namespace ASP.NET_MVC_Forum.Data.Constants
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
            public const int TitleMaxLength = 100;

            public const int TitleMinLength = 10;

            public const int HtmlContentMinLength = 100;
        }

        public class ReportConstants
        {
            public const int ReportReasonMinLength = 10;

            public const int ReportReasonMaxLength = 10000;
        }

        public class CommentConstants
        {
            public const int ContentMaxLength = 500;

            public const int ContentMinLength = 1;
        }

        public class ChatConstants
        {
            public const int ChatMessageMinLength = 1;
            public const int ChatMessageMaxLength = 10000;
        }
    }
}