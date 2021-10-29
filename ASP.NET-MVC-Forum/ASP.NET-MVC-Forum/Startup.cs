namespace ASP.NET_MVC_Forum
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Infrastructure.Extensions;
    using ASP.NET_MVC_Forum.Services.Category;
    using ASP.NET_MVC_Forum.Services.Comment;
    using ASP.NET_MVC_Forum.Services.Post;
    using ASP.NET_MVC_Forum.Services.Report;
    using ASP.NET_MVC_Forum.Services.User;
    using Ganss.XSS;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 10;

            }).AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.Zero;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential 
                // cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                // requires using Microsoft.AspNetCore.Http;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddAutoMapper(typeof(Startup));

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
            });

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IPostService, PostService>();
            services.AddTransient<ICommentService, CommentService>();
            services.AddTransient<IReportService, ReportService>();
            services.AddSingleton<IHtmlSanitizer>(s => new HtmlSanitizer());
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
            });
            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.PrepareDatabase();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseHttpsRedirection();
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            }
            app.UseCookiePolicy();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "AreaAdminHome",
                    pattern: "/{area:exists}/{controller=Home}/{action=Index}");

                endpoints.MapControllerRoute(
                      name: "AreaAdminReports",
                     pattern: "/{area:exists}/{controller=Reports}/{action=Index}/{reportStatus}");

                endpoints.MapControllerRoute(
                      name: "AreaAdminUsers",
                     pattern: "/{area:exists}/{controller=Users}/{action=Index}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");


                endpoints.MapControllerRoute(
                name: "API",
                pattern: "{API}/{controller=Comments}");


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
                pattern: "/Posts/ViewPost/{postId}/{postTitle}",
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

                endpoints.MapRazorPages();
            });
        }
    }
}
