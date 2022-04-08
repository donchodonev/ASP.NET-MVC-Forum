namespace ASP.NET_MVC_Forum.Domain.Exceptions
{
    public class InvalidRoleException : AppException
    {
        public InvalidRoleException()
        {
        }

        public InvalidRoleException(string message) : base(message)
        {
        }

        public InvalidRoleException(string message, params object[] args) : base(message, args)
        {
        }
    }
}
