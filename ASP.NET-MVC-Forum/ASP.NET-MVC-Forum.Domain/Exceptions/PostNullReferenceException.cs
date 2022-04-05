namespace ASP.NET_MVC_Forum.Domain.Exceptions
{
    public class PostNullReferenceException : AppException
    {
        public PostNullReferenceException()
        {
        }

        public PostNullReferenceException(string message) : base(message)
        {
        }

        public PostNullReferenceException(string message, params object[] args) : base(message, args)
        {
        }
    }
}
