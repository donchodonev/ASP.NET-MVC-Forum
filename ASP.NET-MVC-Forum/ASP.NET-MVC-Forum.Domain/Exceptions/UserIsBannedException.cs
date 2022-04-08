namespace ASP.NET_MVC_Forum.Domain.Exceptions
{
    public class UserIsBannedException : AppException
    {
        public UserIsBannedException()
        {
        }

        public UserIsBannedException(string message) : base(message)
        {
        }

        public UserIsBannedException(string message, params object[] args) : base(message, args)
        {
        }
    }
}
