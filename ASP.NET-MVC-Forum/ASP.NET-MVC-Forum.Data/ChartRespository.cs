namespace ASP.NET_MVC_Forum.Data
{
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using System;
    using System.Linq;

    using static ASP.NET_MVC_Forum.Domain.Constants.ColorConstants;

    public class ChartRespository : IChartRepository
    {
        private readonly IMapper mapper;

        public ChartRespository(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public IQueryable<T> GetStatsAs<T>(int count, IQueryable<Post> posts)
        {
            return TakeValidCountOf<Post>(posts, count)
                    .ProjectTo<T>(mapper.ConfigurationProvider);
        }

        public IQueryable<T> GetStatsAs<T>(int count, IQueryable<Category> categories)
        {
            return TakeValidCountOf<Category>(categories, count)
                    .ProjectTo<T>(mapper.ConfigurationProvider);
        }

        private IQueryable<T> TakeValidCountOf<T>(IQueryable<T> items, int requestedCount)
        {
            int itemsTotalCount = items.Count();

            int lowestCountBetweenTotalPostCountAndTheCountOfColors =
                Math.Min(itemsTotalCount,
                Colors.Length);

            int lowestCountBetweenTotalPostCountAndTheCountOfColorsAndRequestedPostsCount =
                Math.Min(
                    lowestCountBetweenTotalPostCountAndTheCountOfColors,
                    requestedCount);

            if (lowestCountBetweenTotalPostCountAndTheCountOfColorsAndRequestedPostsCount >= requestedCount)
            {
                return items
                        .Take(requestedCount);
            }

            return items
                    .Take(lowestCountBetweenTotalPostCountAndTheCountOfColorsAndRequestedPostsCount);
        }
    }
}
