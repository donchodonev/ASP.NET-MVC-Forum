namespace ASP.NET_MVC_Forum.Domain.Exceptions
{
    public class InvalidUsernameException : AppException
    {
        public InvalidUsernameException()
        {
        }

        public InvalidUsernameException(string message) : base(message)
        {
        }

        public InvalidUsernameException(string message, params object[] args) : base(message, args)
        {
        }
    }
}
