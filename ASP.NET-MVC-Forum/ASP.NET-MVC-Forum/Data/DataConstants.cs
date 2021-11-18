using ASP.NET_MVC_Forum.Services.Category;
using System.Collections.Generic;
using System.Linq;

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

        public class ChatConstants
        {
            public const int ChatMessageMinLength = 1;
            public const int ChatMessageMaxLength = 10000;
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
            public const string Navy = "rgb(0,0,128,0.4)";
            public const string Blue = "rgb(0,0,255,0.4)";
            public const string Green = "rgb(0,128,0,0.4)";
            public const string Teal = "rgb(0,128,128,0.4)";
            public const string Lime = "rgb(0,255,0,0.4)";
            public const string Aqua = "rgb(0,255,255,0.4)";
            public const string Maroon = "rgb(128,0,0,0.4)";
            public const string Purple = "rgb(128,0,128,0.4)";
            public const string Olive = "rgb(128,0,128,0.4)";
            public const string Yellow = "rgb(255,255,0,0.4)";
        }

        public class PostSortConstants
        {
            public static IReadOnlyDictionary<int, string> GetSortOptions()
            {
                return new Dictionary<int, string>()
                 {
                      {0,"Date" },
                      { 1,"Post Title"}
                 };
            }

            public static IReadOnlyDictionary<int, string> GetOrderType()
            {
                return new Dictionary<int, string>()
                {
                    { 0,"Descending"},
                    {1,"Ascending" }
                };
            }
        }

        public class PostFilterConstants
        {
            /// <summary>
            /// Returns all existing post categories from the database PLUS the fictional category "All" prepended as the zero-index element
            /// </summary>
            /// <param name="categoryService"></param>
            /// <returns></returns>
            public static IReadOnlyCollection<string> GetCategories(ICategoryService categoryService)
            {
                return categoryService
                    .GetCategoryNames()
                    .Prepend("All")
                    .ToList()
                    .AsReadOnly();
            }
        }

        public class PostViewCountOptionsConstants
        {
            public static IReadOnlyCollection<int> GetViewCountOptions()
            {
                return new List<int> { 10, 20, 30 }.AsReadOnly();
            }
        }
    }
}