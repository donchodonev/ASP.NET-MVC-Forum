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

        public class RoleConstants
        {
            public const string AdminRoleName = "Administrator";

            public const string ModeratorRoleName = "Moderator";

            public const string AdminOrModerator = AdminRoleName + "," + ModeratorRoleName;
        }

        public class DateTimeFormat
        {
            public const string DateFormat = "MM/dd/yyyy";

            public const string DateAndTimeFormat = "MM/dd/yyyy HH:mm";
        }

        public class WebConstants
        {
            public const string AvatarWebPath = "/avatar/";
            public const string AvatarDirectoryPath = "\\wwwroot\\avatar\\";
        }

        public class AllowedImageExtensions
        {
            public const string JPEG = ".jpeg";
            public const string JPG = ".jpg";
            public const string PNG = ".png";
            public const string WEBP = ".webp";
            public const string BMP = ".bmp";
            public string[] AllowedImageExtensionsArray = new string[5] { JPEG, JPG, PNG, WEBP, BMP };
        }
    }
}
