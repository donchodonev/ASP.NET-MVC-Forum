namespace ASP.NET_MVC_Forum.Domain.Exceptions
{
    public class PostReportDoesNotExistException : AppException
    {
        public PostReportDoesNotExistException()
        {
        }

        public PostReportDoesNotExistException(string message) : base(message)
        {
        }

        public PostReportDoesNotExistException(string message, params object[] args) : base(message, args)
        {
        }
    }
}
