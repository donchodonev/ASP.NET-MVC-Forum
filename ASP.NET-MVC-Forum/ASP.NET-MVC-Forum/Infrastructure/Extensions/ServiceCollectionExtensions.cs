namespace ASP.NET_MVC_Forum.Infrastructure.Extensions
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Services.Business.Censor;
    using ASP.NET_MVC_Forum.Services.Business.EmailSender;
    using ASP.NET_MVC_Forum.Services.Business.HtmlManipulator;
    using ASP.NET_MVC_Forum.Services.Business.Post;
    using ASP.NET_MVC_Forum.Services.Business.PostReport;
    using ASP.NET_MVC_Forum.Services.Business.UserAvatar;
    using ASP.NET_MVC_Forum.Services.Chat;
    using ASP.NET_MVC_Forum.Services.Comment;
    using ASP.NET_MVC_Forum.Services.CommentReport;
    using ASP.NET_MVC_Forum.Services.Data.Category;
    using ASP.NET_MVC_Forum.Services.Data.Chart;
    using ASP.NET_MVC_Forum.Services.Data.Post;
    using ASP.NET_MVC_Forum.Services.Data.PostReport;
    using ASP.NET_MVC_Forum.Services.User;
    using ASP.NET_MVC_Forum.Services.Data.Vote;

    using Ganss.XSS;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using ProfanityFilter;
    using ProfanityFilter.Interfaces;

    using System;
    using ASP.NET_MVC_Forum.Services.Business.Vote;

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

        public static void SetupDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IPostBusinessService, PostBusinessService>();
            services.AddTransient<IPostDataService, PostDataService>();
            services.AddTransient<ICommentService, CommentService>();
            services.AddTransient<IPostReportDataService, PostReportDataService>();
            services.AddTransient<IPostReportBusinessService, PostReportBusinessService>();
            services.AddTransient<ICensorService, CensorService>();
            services.AddTransient<ICommentReportService, CommentReportService>();
            services.AddTransient<IProfanityFilter, ProfanityFilter>();
            services.AddTransient<IVoteBusinessService, VoteBusinessService>();
            services.AddTransient<IVoteDataService, VoteDataService>();
            services.AddTransient<IUserAvatarService, UserAvatarService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IChartDataService, ChartDataService>();
            services.AddTransient<IHtmlSanitizer>(s => new HtmlSanitizer());
            services.AddTransient<IChatService, ChatService>();
            services.AddTransient<IHtmlManipulator, HtmlManipulator>();
            services.AddSingleton(configuration);
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
            });
            services.AddMemoryCache();
        }

        public static void AddDatabase (this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(
                   configuration.GetConnectionString("DefaultConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();
        }

        public static void SetupSendgrindConnectionKey(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AuthMessageSenderOptions>(configuration);
        }

        public static void ConfigureSecurityStampValidation(this IServiceCollection services)
        {
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.Zero;
            });
        }

        public static void ConfigureCookiePolicy(this IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
        }

        public static void SetupControllersWithViews(this IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
            });
        }
    }
}
