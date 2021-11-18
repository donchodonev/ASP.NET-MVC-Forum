namespace ASP.NET_MVC_Forum.Infrastructure.Extensions
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Services.Category;
    using ASP.NET_MVC_Forum.Services.Comment;
    using ASP.NET_MVC_Forum.Services.CommentReport;
    using ASP.NET_MVC_Forum.Services.Post;
    using ASP.NET_MVC_Forum.Services.PostReport;
    using ASP.NET_MVC_Forum.Services.User;
    using ASP.NET_MVC_Forum.Services.UserAvatarService;
    using ASP.NET_MVC_Forum.Services.Vote;
    using Ganss.XSS;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using ProfanityFilter.Interfaces;
    using ProfanityFilter;
    using ASP.NET_MVC_Forum.Services.EmailSender;
    using ASP.NET_MVC_Forum.Services.Chart;
    using ASP.NET_MVC_Forum.Services.Chat;

    public static class ServiceCollectionExtensions
    {
        public static void DefaultIdentitySetup(this IServiceCollection services)
        {
            services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 10;

            }).AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
        }

        public static void FacebookLoginProviderSetup(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = configuration["Authentication:Facebook:AppSecret"];
            });
        }

        public static void DependenciesSetup(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IPostService, PostService>();
            services.AddTransient<ICommentService, CommentService>();
            services.AddTransient<IPostReportService, PostReportService>();
            services.AddTransient<ICommentReportService, CommentReportService>();
            services.AddTransient<IProfanityFilter, ProfanityFilter>();
            services.AddTransient<IVoteService, VoteService>();
            services.AddTransient<IUserAvatarService, UserAvatarService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IChartService, ChartService>();
            services.AddTransient<IHtmlSanitizer>(s => new HtmlSanitizer());
            services.AddTransient<IChatService, ChatService>();
            services.AddSingleton(configuration);
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
            });
            services.AddMemoryCache();
        }
    }
}
