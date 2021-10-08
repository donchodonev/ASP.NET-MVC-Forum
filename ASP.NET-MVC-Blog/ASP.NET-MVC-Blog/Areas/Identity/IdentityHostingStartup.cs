using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(ASP.NET_MVC_Blog.Areas.Identity.IdentityHostingStartup))]
namespace ASP.NET_MVC_Blog.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}