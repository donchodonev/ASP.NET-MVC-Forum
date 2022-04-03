namespace ASP.NET_MVC_Forum
{
    using ASP.NET_MVC_Forum.Web.Extensions;
    using ASP.NET_MVC_Forum.Web.Infrastructure.Extensions;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabase(Configuration);
            services.DefaultIdentitySetup();
            services.FacebookLoginProviderSetup(Configuration);
            services.SetupSendgrindConnectionKey(Configuration);
            services.ConfigureSecurityStampValidation();
            services.ConfigureCookiePolicy();
            services.AddSignalR();
            services.SetupControllersWithViews();
            services.SetupDependencyInjection(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.PrepareDatabaseAsync().Wait();

                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseHttpsRedirection();
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseCookiePolicy();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.SetupRouting();
        }
    }
}
