namespace ASP.NET_MVC_Forum.Web.Data.Constants
{
    using System.Collections.Generic;

    public class PostSortConstants
    {
        public static IReadOnlyDictionary<int, string> GetSortOptions()
        {
            return new Dictionary<int, string>()
                 {
                      {0,"Date" },
                      { 1,"Post Title"}
                 };
        }

        public static IReadOnlyDictionary<int, string> GetOrderType()
        {
            return new Dictionary<int, string>()
                {
                    { 0,"Descending"},
                    {1,"Ascending" }
                };
        }
    }
}
