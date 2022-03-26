namespace ASP.NET_MVC_Forum.Domain.Models
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static ASP.NET_MVC_Forum.Domain.Constants.PostViewCountOptions;

    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {

            if (pageSize == 0)
            {
                pageSize = GetViewCountOptions().First();
            }

            int validPageIndex = GetValidPageIndex(pageIndex);

            var count = await source.CountAsync();
            var items = await source.Skip((validPageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, validPageIndex, pageSize);
        }

        public static int GetValidPageIndex(int pageIndex)
        {
            return Math.Max(pageIndex, 1);
        }
    }
}
