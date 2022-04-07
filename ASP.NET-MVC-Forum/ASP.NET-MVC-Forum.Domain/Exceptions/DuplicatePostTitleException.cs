namespace ASP.NET_MVC_Forum.Domain.Exceptions
{
    public class DuplicatePostTitleException : AppException
    {
        public DuplicatePostTitleException()
        {
        }

        public DuplicatePostTitleException(string message) : base(message)
        {
        }

        public DuplicatePostTitleException(string message, params object[] args) : base(message, args)
        {
        }
    }
}
