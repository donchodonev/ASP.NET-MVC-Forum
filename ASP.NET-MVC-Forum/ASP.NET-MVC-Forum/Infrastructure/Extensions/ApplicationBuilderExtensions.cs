﻿namespace ASP.NET_MVC_Forum.Infrastructure.Extensions
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Models;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using static Data.DataConstants.RoleConstants;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder PrepareDatabase(
            this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var services = serviceScope.ServiceProvider;

            SeedCategories(services);
            SeedAdministrator(services);

            return app;
        }

        private static void SeedCategories(IServiceProvider services)
        {
            var data = services.GetRequiredService<ApplicationDbContext>();

            if (data.Categories.Any())
            {
                return;
            }

            data.Categories.AddRange(new[]
            {
                new Category{ Name = "Guides", ImageUrl = "https://guide.directindustry.com/wp-content/themes/framework/media/DI-icon.png"},
                new Category{ Name = "Tech", ImageUrl = "https://news.cgtn.com/news/2020-11-02/Analysis-China-is-betting-on-science-and-tech-like-never-before-V68V871ula/img/871ca9ce8b9941088260b6ed4ced4eeb/871ca9ce8b9941088260b6ed4ced4eeb.jpeg"},
                new Category{ Name = "Sports", ImageUrl = "https://pohvalno.info/wp-content/uploads/2018/08/sport-777.jpg"},
                new Category{ Name = "Pets", ImageUrl = "https://i2-prod.walesonline.co.uk/incoming/article20715699.ece/ALTERNATES/s615/2_Precious_Pets_2-1.jpg"},
                new Category{ Name = "World", ImageUrl = "https://www.catalyticconverterrecycling.org/wp-content/uploads/2020/06/world-catalytic-converter.jpg"},
                new Category{ Name = "Coronavirus", ImageUrl = "https://www.nps.gov/aboutus/news/images/CDC-coronavirus-image-23311-for-web.jpg?maxwidth=650&autorotate=false"},
                new Category{ Name = "Celebrity", ImageUrl = "https://www.nami.org/NAMI/media/NAMI-Media/BlogImageArchive/2016/celebrities-blog.jpeg"},
            });

            data.SaveChanges();
        }

        private static void SeedAdministrator(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var data = services.GetRequiredService<ApplicationDbContext>();

            Task.Run(async () =>
            {
                if (await roleManager.RoleExistsAsync(AdminRoleName))
                {
                    return;
                }

                var role = new IdentityRole() { Name = AdminRoleName };
                await roleManager.CreateAsync(role);

                var user = new IdentityUser()
                {
                    UserName = "admin",
                    Email = "donevdoncho92@gmail.com",
                };

                await userManager.CreateAsync(user, "123456");
                await userManager.AddToRoleAsync(user, AdminRoleName);

                var baseUser = new User()
                {
                    FirstName = "Doncho",
                    LastName = "Donev",
                    IdentityUserId = user.Id
                };

                data.BaseUsers.Add(baseUser);

                data.SaveChanges();
            })
                .GetAwaiter()
                .GetResult();
        }
    }
}