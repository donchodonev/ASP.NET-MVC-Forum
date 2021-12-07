namespace ASP.NET_MVC_Forum.Data.Constants
{
    using static ASP.NET_MVC_Forum.Data.Constants.RoleConstants;
    public class ClientMessage
    {
        public class Error
        {
            public const string PostLengthTooSmall = "The length of the post must be longer than 100 symbols";
            public const string UserDoesNotExist = "User does NOT exist !";
            public const string YouAreNotTheAuthor = "You are not the author of this post";
            public const string SuchAPostDoesNotExist = "Such a post does not exist";
            public const string PostRemainsUnchanged = "There were no changes made to the post's title, body or category";
            public const string DuplicatePostName = "A post with the same title already exists";
            public const string CannotFurtherDemote = "User cannot be further demoted";
            public const string UserIsAlreadyAModerator = "User is already a moderator";
            public const string UserIsAlreadyBanned = "User is already a banned";
            public const string UsernameTooShort = "Username must be at least 4 symbols long";
            public const string ReportDoesNotExist = "A report with such an ID does not exist";
        }
        public class Success
        {
            public const string ReportThankYouMessage = "Thank you for your report, our moderators will review it as quickly as possible !";
            public const string PostSuccessfullyDeleted = "Your post has been successfully deleted";
            public const string UserSucessfullyBanned = "The user has been successfully banned indefinitely";
            public const string UserSucessfullyUnBanned = "The user has been successfully unbanned";
            public const string UserSuccessfullyDemoted = "The user has been successfully demoted";
            public const string UserSuccessfullyPromoted = "The user has been successfully promoted to " + ModeratorRoleName;
            public const string ReportResolved = "Report has been marked as resolved !";
            public const string ReportRestored = "Report has been successfully restored !";
            public const string CommentReportCensored = "Comment has been successfully censored !";
            public const string CommentReportCensoredAndResolved = "The comment has been successfully censored and report was marked as resolved";
            public const string CommentCensoredAndResolved = "The comment has been successfully censored and report was marked as resolved";
            public const string PostCensored = "The post has been successfully censored";
            public const string PostCensoredAndResolved = "The post has been successfully censored and all of it's reports were marked as resolved";
        }
        public class MessageType
        {
            public const string ErrorMessage = "ErrorMessage";
            public const string SuccessMessage = "SuccessMessage";
            public const string GenericMessage = "GenericMessage";
        }
    }
}
