namespace ASP.NET_MVC_Forum.Domain.Constants
{
    using System.Collections.Generic;

    public static class PostSortConstants
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
