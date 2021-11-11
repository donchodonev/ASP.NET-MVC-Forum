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
            public const string AvatarURL = "/avatar/defaultUserImage.png";
        }

        public class ImageConstants
        {
            /// <summary>
            /// Image maximum size in bytes
            /// </summary>
            public const long ImageMaxSize = 5242880;
            public const string JPEG = ".jpeg";
            public const string JPG = ".jpg";
            public const string PNG = ".png";
            public const string BMP = ".bmp";
        }

        public class ColorConstants
        {
            /// <summary>
            /// Color RGB Values
            /// </summary>
            public const string Navy = "rgb(0,0,128)";
            public const string Blue = "rgb(0,0,255)";
            public const string Green = "rgb(0,128,0)";
            public const string Teal = "rgb(0,128,128)";
            public const string Lime = "rgb(0,255,0)";
            public const string Aqua = "rgb(0,255,255)";
            public const string Maroon = "rgb(128,0,0)";
            public const string Purple = "rgb(128,0,128)";
            public const string Olive = "rgb(128,0,128)";
            public const string Yellow = "rgb(255,255,0)";
        }
    }
}
