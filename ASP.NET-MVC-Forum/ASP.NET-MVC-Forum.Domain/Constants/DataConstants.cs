namespace ASP.NET_MVC_Forum.Domain.Constants
{
    public static class DataConstants
    {
        public static class UserConstants
        {
            public const int FIRST_NAME_MAX_LENGTH = 15;

            public const int LAST_NAME_MAX_LENGTH = 15;

            public const int FIRST_NAME_MIN_LENGTH = 2;

            public const int LAST_NAME_MIN_LENGTH = 2;

            public const int AGE_FLOOR = 0;

            public const int AGE_CEILING = 120;

            public const int USERNAME_MAX_LENGTH = 20;

            public const int USERNAME_MIN_LENGTH = 4;
        }

        public static class CategoryConstants
        {
            public const int NAME_MAX_LENGTH = 100;
        }

        public static class PostConstants
        {
            public const int TITLE_MAX_LENGTH = 100;

            public const int TITLE_MIN_LENGTH = 10;

            public const int HTML_CONTENT_MIN_LENGTH = 100;
        }

        public static class ReportConstants
        {
            public const int REPORT_REASON_MIN_LENGTH = 10;

            public const int REPORT_REASON_MAX_LENGTH = 10000;
        }

        public static class CommentConstants
        {
            public const int CONTENT_MAX_LENGTH = 500;

            public const int CONTENT_MIN_LENGTH = 1;
        }

        public static class ChatConstants
        {
            public const int CHAT_MESSAGE_MIN_LENGTH = 1;
            public const int CHAT_MESSAGE_MAX_LENGTH = 10000;
        }
    }
}