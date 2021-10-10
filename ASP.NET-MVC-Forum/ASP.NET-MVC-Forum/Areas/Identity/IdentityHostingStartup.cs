using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(ASP.NET_MVC_Forum.Areas.Identity.IdentityHostingStartup))]
namespace ASP.NET_MVC_Forum.Areas.Identity
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