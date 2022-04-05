﻿namespace ASP.NET_MVC_Forum.Web.Extensions
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Entities;
    using ASP.NET_MVC_Forum.Web.Hubs;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using static ASP.NET_MVC_Forum.Domain.Constants.RoleConstants;

    public static class ApplicationBuilderExtensions
    {
        public static async Task<IApplicationBuilder> PrepareDatabaseAsync(
            this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var services = serviceScope.ServiceProvider;

            await SeedCategoriesAsync(services);
            await SeedRolesAsync(services);
            await SeedAdministratorAsync(services);
            await SeedPostsAsync(services);

            return app;
        }

        public static void SetupRouting(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "AreaAdminHome",
                    pattern: "/{area:exists}/{controller=Home}/{action=Index}");

                endpoints.MapControllerRoute(
                      name: "AreaAdminPostReports",
                     pattern: "/{area:exists}/{controller=PostReports}/{action=Index}/{reportStatus}");

                endpoints.MapControllerRoute(
                      name: "AreaAdminCommentReports",
                     pattern: "/{area:exists}/{controller=PostReports}/{action=Index}/{reportStatus}");

                endpoints.MapControllerRoute(
                      name: "AreaAdminUsers",
                     pattern: "/{area:exists}/{controller=Users}/{action=Index}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                name: "Post Delete",
                pattern: "/Posts/Delete/{postId}/{postTitle}",
                defaults: new
                {
                    controller = "Posts",
                    action = "Delete",
                });

                endpoints.MapControllerRoute(
                name: "Post view",
                pattern: "/Posts/ViewPost/{postId}",
                defaults: new
                {
                    controller = "Posts",
                    action = "ViewPost",
                });

                endpoints.MapControllerRoute(
                name: "Post report",
                pattern: "/Posts/Report/{postId}",
                defaults: new
                {
                    controller = "Posts",
                    action = "Report",
                });

                endpoints.MapControllerRoute(
                    name: "Category Info",
                    pattern: "/Categories/CategoryContent/{categoryId}/{categoryName}",
                    defaults: new
                    {
                        controller = "Categories",
                        action = "CategoryContent",
                    });

                endpoints.MapControllerRoute(
                    name: "Add post",
                    pattern: "/Posts/Add/{title?}/{categoryId?}",
                    defaults: new
                    {
                        controller = "Posts",
                        action = "Add",
                    });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Chat}/{action=ChatConversation}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Chat}/{action=SelectUser}/{username?}");

                endpoints.MapHub<ChatHub>("/mychat");

                endpoints.MapRazorPages();
            });
        }

        private static async Task SeedCategoriesAsync(IServiceProvider services)
        {
            var data = services.GetRequiredService<ApplicationDbContext>();

            if (await data.Categories.AnyAsync())
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

            await data.SaveChangesAsync();
        }

        private static async Task SeedAdministratorAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ExtendedIdentityUser>>();

            await userManager.CreateAsync(new ExtendedIdentityUser()
            {
                FirstName = "Doncho",
                LastName = "Donev",
                Email = "donevdoncho92@gmail.com",
                UserName = "admin",
                EmailConfirmed = true
            }, "d123456789D@");
        }

        private static async Task SeedRolesAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            if (await roleManager.RoleExistsAsync(AdminRoleName))
            {
                return;
            }

            if (await roleManager.RoleExistsAsync(ModeratorRoleName))
            {
                return;
            }

            var adminRole = new IdentityRole() { Name = AdminRoleName };

            var moderatorRole = new IdentityRole() { Name = ModeratorRoleName };

            await roleManager.CreateAsync(adminRole);

            await roleManager.CreateAsync(moderatorRole);
        }

        private static async Task SeedPostsAsync(IServiceProvider services)
        {
            var db = services.GetRequiredService<ApplicationDbContext>();

            var userRepo = services.GetRequiredService<IUserRepository>();

            var adminId = await userRepo
                    .GetAll()
                    .Select(x => x.Id)
                    .FirstAsync();

            if (await db.Posts.AnyAsync())
            {
                return;
            }

            var categories = await db.Categories.ToListAsync();

            var posts = new List<Post>();

            for (int i = 0; i < categories.Count; i++)
            {
                posts.Add(new Post()
                {
                    Title = $"{i}",
                    CategoryId = categories[i].Id,
                    Category = categories[i],
                    UserId = adminId,
                    HtmlContent = @"
                    Lorem ipsum dolor sit amet,
                    consectetur adipiscing elit,
                    sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
                });
            }

            db.Posts.AddRange(posts);

            await db.SaveChangesAsync();
        }
    }
}