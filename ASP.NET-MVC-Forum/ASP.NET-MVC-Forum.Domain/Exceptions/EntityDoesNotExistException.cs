namespace ASP.NET_MVC_Forum.Domain.Exceptions
{
    public class EntityDoesNotExistException : AppException
    {
        public EntityDoesNotExistException()
        {
        }

        public EntityDoesNotExistException(string message) : base(message)
        {
        }

        public EntityDoesNotExistException(string message, params object[] args) : base(message, args)
        {
        }
    }
}
