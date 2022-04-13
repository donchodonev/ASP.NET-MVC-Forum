namespace ASP.NET_MVC_Forum.Domain.Exceptions
{
    public class InvalidReportStatusException : AppException
    {
        public InvalidReportStatusException()
        {
        }

        public InvalidReportStatusException(string message) : base(message)
        {
        }

        public InvalidReportStatusException(string message, params object[] args) : base(message, args)
        {
        }
    }
}
