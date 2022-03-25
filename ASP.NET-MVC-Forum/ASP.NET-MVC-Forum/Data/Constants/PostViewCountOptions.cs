namespace ASP.NET_MVC_Forum.Web.Data.Constants
{
    using System.Collections.Generic;

    public class PostViewCountOptions
    {
        public static IReadOnlyCollection<int> GetViewCountOptions()
        {
            return new List<int> { 10, 20, 30 }.AsReadOnly();
        }
    }
}
