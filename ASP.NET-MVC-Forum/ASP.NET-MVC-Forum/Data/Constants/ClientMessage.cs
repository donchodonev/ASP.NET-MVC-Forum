namespace ASP.NET_MVC_Forum.Data.Constants
{
    public class ClientMessage
    {
        public class Error
        {
            public const string PostLengthTooSmall = "The length of the post must be longer than 100 symbols";
            public const string UserDoesNotExist = "User does NOT exist !";
            public const string YouAreNotTheAuthor = "You are not the author of this post";
            public const string SuchAPostDoesNotExist = "Such a post does not exist";
        }
        public class Success
        {
            public const string ReportThankYouMessage = "Thank you for your report, our moderators will review it as quickly as possible !";
        }
        public class MessageType
        {
            public const string ErrorMessage = "ErrorMessage";
            public const string SuccessMessage = "SuccessMessage";
            public const string GenericMessage = "GenericMessage";
        }
    }
}
