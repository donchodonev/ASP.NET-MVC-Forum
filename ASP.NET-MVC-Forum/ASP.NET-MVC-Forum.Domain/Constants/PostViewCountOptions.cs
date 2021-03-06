namespace ASP.NET_MVC_Forum.Domain.Constants
{
    using System.Collections.Generic;

    public static class PostViewCountOptions
    {
        public static IReadOnlyCollection<int> GetViewCountOptions()
        {
            return new List<int> { 10, 20, 30 }.AsReadOnly();
        }
    }
}
