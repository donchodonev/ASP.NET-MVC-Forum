namespace ASP.NET_MVC_Forum.Domain.Constants
{
    using static ASP.NET_MVC_Forum.Domain.Constants.RoleConstants;

    public class ClientMessage
    {
        public class Error
        {
            public const string POST_LENGTH_TOO_SMALL = "The length of the post must be longer than 100 symbols";
            public const string USER_DOES_NOT_EXIST = "User does NOT exist !";
            public const string YOU_ARE_NOT_THE_AUTHER = "You are not the author of this post";
            public const string POST_IS_UNCHANGED = "There were no changes made to the post's title, body or category";
            public const string DUPLICATE_POST_NAME = "A post with the same title already exists";
            public const string CANNOT_FURTHER_DEMOTE = "User cannot be further demoted";
            public const string USER_IS_MODERATOR_ALREADY = "User is already a moderator";
            public const string USER_ALREADY_BANNED = "User is already a banned";
            public const string USERNAME_TOO_SHORT = "Username must be at least 4 symbols long";
            public const string REPORT_DOES_NOT_EXIST = "A report with such an ID does not exist";
            public const string POST_DOES_NOT_EXIST = "The post doesn't exist";
            public const string POST_DID_NOT_CHANGE = "There were no changes made";
        }
        public class Success
        {
            public const string REPORT_THANK_YOU_MESSAGE = "Thank you for your report, our moderators will review it as quickly as possible !";
            public const string POST_DELETED = "Your post has been successfully deleted";
            public const string USER_BANNED = "The user has been successfully banned indefinitely";
            public const string USER_UNBANNED = "The user has been successfully unbanned";
            public const string USER_DEMOTED = "The user has been successfully demoted";
            public const string USER_PROMOTED = "The user has been successfully promoted to " + ModeratorRoleName;
            public const string REPORT_RESOLVED = "Report has been marked as resolved !";
            public const string REPORT_RESTORED = "Report has been successfully restored !";
            public const string COMMENT_REPORT_CENSORED = "Comment has been successfully censored !";
            public const string COMMENT_REPORT_CENSORED_AND_RESOLVED = "The comment has been successfully censored and report was marked as resolved";
            public const string POST_CENSORED = "The post has been successfully censored";
            public const string POST_CENSORED_AND_RESOLVED = "The post has been successfully censored and all of it's reports were marked as resolved";
        }
        public class MessageType
        {
            public const string ERROR_MESSAGE = "ErrorMessage";
            public const string SUCCESS_MESSAGE = "SuccessMessage";
            public const string GENERIC_MESSAGE = "GenericMessage";
        }
    }
}
