namespace ASP.NET_MVC_Forum.Domain.Exceptions
{
    public class NoUpdatesMadeException : AppException
    {
        public NoUpdatesMadeException()
        {
        }

        public NoUpdatesMadeException(string message) : base(message)
        {
        }

        public NoUpdatesMadeException(string message, params object[] args) : base(message, args)
        {
        }
    }
}
