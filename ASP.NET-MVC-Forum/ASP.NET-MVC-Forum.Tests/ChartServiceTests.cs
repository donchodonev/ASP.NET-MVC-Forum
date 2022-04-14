namespace ASP.NET_MVC_Forum.Tests
{
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Infrastructure.MappingProfiles;

    using AutoMapper;

    using Microsoft.EntityFrameworkCore;

    using NUnit.Framework;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ChartServiceTests
    {
        private PostMappingProfile postMappingProfile;
        private CategoryMappingProfile categoryMappingProfile;
        private MapperConfiguration mapperConfiguration;
        private IMapper mapper;
        private DbContextOptions<ApplicationDbContext> dbContextOptions;
        private ApplicationDbContext dbContext;
        private IPostRepository postRepository;
        private ICategoryRepository categoryRepository;
        private IChartService chartService;

        [SetUp]
        public async Task SetUp()
        {
            postMappingProfile = new PostMappingProfile();
            categoryMappingProfile = new CategoryMappingProfile();
            mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfiles(new Profile[] { categoryMappingProfile, postMappingProfile }));
            mapper = new Mapper(mapperConfiguration);
            dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("ForumDb").Options;
            dbContext = new ApplicationDbContext(dbContextOptions);
            postRepository = new PostRepository(dbContext);
            categoryRepository = new CategoryRepository(dbContext, mapper);
            chartService = new ChartService(postRepository, categoryRepository, mapper);
            await AddCategoriesToDatabaseAsync();
            await AddPostsToDatabaseAsync();
        }


        [Test]
        public async Task GetMostCommentedPostsChartDataAsync_ShouldReturnRequestedCountOfModels_WhenGivenPositiveCount()
        {
            var models = await chartService.GetMostCommentedPostsChartDataAsync(7);

            int expectedModelsCount = 7;

            int actualModelsCount = models.Count;

            Assert.AreEqual(expectedModelsCount, actualModelsCount);
        }

        [Test]
        public async Task GetMostCommentedPostsChartDataAsync_ShouldReturn_OneModel_WhenGivenNegativeCount()
        {
            var models = await chartService.GetMostCommentedPostsChartDataAsync(-1);

            int expectedModelsCount = 1;

            int actualModelsCount = models.Count;

            Assert.AreEqual(expectedModelsCount, actualModelsCount);
        }

        [Test]
        public async Task GetMostCommentedPostsChartDataAsync_ShouldReturn_OneModel_WhenGivenCountOfZero()
        {
            var models = await chartService.GetMostCommentedPostsChartDataAsync(0);

            int expectedModelsCount = 1;

            int actualModelsCount = models.Count;

            Assert.AreEqual(expectedModelsCount, actualModelsCount);
        }

        [Test]
        public async Task GetMostLikedPostsChartDataAsync_ShouldReturnRequestedCountOfModels_WhenGivenPositiveCount()
        {
            var models = await chartService.GetMostLikedPostsChartDataAsync(7);

            int expectedModelsCount = 7;

            int actualModelsCount = models.Count;

            Assert.AreEqual(expectedModelsCount, actualModelsCount);
        }

        [Test]
        public async Task GetMostLikedPostsChartDataAsync_ShouldReturn_OneModel_WhenGivenNegativeCount()
        {
            var models = await chartService.GetMostLikedPostsChartDataAsync(-1);

            int expectedModelsCount = 1;

            int actualModelsCount = models.Count;

            Assert.AreEqual(expectedModelsCount, actualModelsCount);
        }

        [Test]
        public async Task GetMostLikedPostsChartDataAsync_ShouldReturn_OneModel_WhenGivenCountOfZero()
        {
            var models = await chartService.GetMostLikedPostsChartDataAsync(0);

            int expectedModelsCount = 1;

            int actualModelsCount = models.Count;

            Assert.AreEqual(expectedModelsCount, actualModelsCount);
        }

        [Test]
        public async Task GetMostReportedPostsChartDataAsync_ShouldReturnRequestedCountOfModels_WhenGivenPositiveCount()
        {
            var models = await chartService.GetMostReportedPostsChartDataAsync(7);

            int expectedModelsCount = 7;

            int actualModelsCount = models.Count;

            Assert.AreEqual(expectedModelsCount, actualModelsCount);
        }

        [Test]
        public async Task GetMostReportedPostsChartDataAsync_ShouldReturn_OneModel_WhenGivenNegativeCount()
        {
            var models = await chartService.GetMostReportedPostsChartDataAsync(-1);

            int expectedModelsCount = 1;

            int actualModelsCount = models.Count;

            Assert.AreEqual(expectedModelsCount, actualModelsCount);
        }

        [Test]
        public async Task GetMostReportedPostsChartDataAsync_ShouldReturn_OneModel_WhenGivenCountOfZero()
        {
            var models = await chartService.GetMostReportedPostsChartDataAsync(0);

            int expectedModelsCount = 1;

            int actualModelsCount = models.Count;

            Assert.AreEqual(expectedModelsCount, actualModelsCount);
        }

        [Test]
        public async Task GetMostPostsPerCategoryAsync_ShouldReturnRequestedCountOfModels_WhenGivenPositiveCount()
        {
            var models = await chartService.GetMostReportedPostsChartDataAsync(7);

            int expectedModelsCount = 7;

            int actualModelsCount = models.Count;

            Assert.AreEqual(expectedModelsCount, actualModelsCount);
        }

        [Test]
        public async Task GetMostPostsPerCategoryAsync_ShouldReturn_OneModel_WhenGivenNegativeCount()
        {
            var models = await chartService.GetMostReportedPostsChartDataAsync(-1);

            int expectedModelsCount = 1;

            int actualModelsCount = models.Count;

            Assert.AreEqual(expectedModelsCount, actualModelsCount);
        }

        [Test]
        public async Task GetMostPostsPerCategoryAsync_ShouldReturn_OneModel_WhenGivenCountOfZero()
        {
            var models = await chartService.GetMostPostsPerCategoryAsync(0);

            int expectedModelsCount = 1;

            int actualModelsCount = models.Count;

            Assert.AreEqual(expectedModelsCount, actualModelsCount);
        }

        private async Task AddCategoriesToDatabaseAsync()
        {
            if (await dbContext.Categories.AnyAsync())
            {
                return;
            }

            dbContext.Categories.AddRange(new[]
            {
                new Category{ Name = "Guides", ImageUrl = "https://guide.directindustry.com/wp-content/themes/framework/media/DI-icon.png"},
                new Category{ Name = "Tech", ImageUrl = "https://news.cgtn.com/news/2020-11-02/Analysis-China-is-betting-on-science-and-tech-like-never-before-V68V871ula/img/871ca9ce8b9941088260b6ed4ced4eeb/871ca9ce8b9941088260b6ed4ced4eeb.jpeg"},
                new Category{ Name = "Sports", ImageUrl = "https://pohvalno.info/wp-content/uploads/2018/08/sport-777.jpg"},
                new Category{ Name = "Pets", ImageUrl = "https://i2-prod.walesonline.co.uk/incoming/article20715699.ece/ALTERNATES/s615/2_Precious_Pets_2-1.jpg"},
                new Category{ Name = "World", ImageUrl = "https://www.catalyticconverterrecycling.org/wp-content/uploads/2020/06/world-catalytic-converter.jpg"},
                new Category{ Name = "Coronavirus", ImageUrl = "https://www.nps.gov/aboutus/news/images/CDC-coronavirus-image-23311-for-web.jpg?maxwidth=650&autorotate=false"},
                new Category{ Name = "Celebrity", ImageUrl = "https://www.nami.org/NAMI/media/NAMI-Media/BlogImageArchive/2016/celebrities-blog.jpeg"},
            });

            await dbContext.SaveChangesAsync();
        }

        private async Task AddPostsToDatabaseAsync()
        {
            if (await dbContext.Posts.AnyAsync())
            {
                return;
            }

            List<Post> posts = new List<Post>();

            for (int i = 0; i < await dbContext.Categories.CountAsync(); i++)
            {
                var random = new Random();

                var categoryIds = await dbContext.Categories.Select(x => x.Id).ToArrayAsync();

                var post = new Post()
                {
                    UserId = "some user id",
                    CategoryId = categoryIds[i],
                    Title = $"Title number {random.Next()}",
                    HtmlContent = @"
                    Lorem ipsum dolor sit amet,
                    consectetur adipiscing elit,
                    sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est."
                };

                posts.Add(post);
            }

            dbContext.Posts.AddRange(posts);

            await dbContext.SaveChangesAsync();
        }
    }
}
