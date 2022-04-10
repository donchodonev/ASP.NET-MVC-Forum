namespace ASP.NET_MVC_Forum.Domain.Exceptions
{
    public class CommentReportDoesNotExistException : AppException
    {
        public CommentReportDoesNotExistException()
        {
        }

        public CommentReportDoesNotExistException(string message) : base(message)
        {
        }

        public CommentReportDoesNotExistException(string message, params object[] args) : base(message, args)
        {
        }
    }
}
