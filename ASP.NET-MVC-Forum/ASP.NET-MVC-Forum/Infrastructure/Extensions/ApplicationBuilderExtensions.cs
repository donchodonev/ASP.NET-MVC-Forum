namespace ASP.NET_MVC_Forum.Infrastructure.Extensions
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
                new Category{ Name = "Guides"},
                new Category{ Name = "Tech News"}
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