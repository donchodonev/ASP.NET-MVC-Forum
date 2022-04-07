namespace ASP.NET_MVC_Forum.Domain.Exceptions
{
    public class NullUserException : AppException
    {
        public NullUserException()
        {
        }

        public NullUserException(string message) : base(message)
        {
        }

        public NullUserException(string message, params object[] args) : base(message, args)
        {
        }
    }
}
