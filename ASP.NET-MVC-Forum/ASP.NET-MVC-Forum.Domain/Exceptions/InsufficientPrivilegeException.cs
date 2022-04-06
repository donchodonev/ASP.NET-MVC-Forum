namespace ASP.NET_MVC_Forum.Domain.Exceptions
{
    public class InsufficientPrivilegeException : AppException
    {
        public InsufficientPrivilegeException()
        {
        }

        public InsufficientPrivilegeException(string message) : base(message)
        {
        }

        public InsufficientPrivilegeException(string message, params object[] args) : base(message, args)
        {
        }
    }
}
