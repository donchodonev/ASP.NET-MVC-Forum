namespace ASP.NET_MVC_Forum.Domain.Exceptions
{
    public class NullCommentException : AppException
    {
        public NullCommentException()
        {
        }

        public NullCommentException(string message) : base(message)
        {
        }

        public NullCommentException(string message, params object[] args) : base(message, args)
        {
        }
    }
}
