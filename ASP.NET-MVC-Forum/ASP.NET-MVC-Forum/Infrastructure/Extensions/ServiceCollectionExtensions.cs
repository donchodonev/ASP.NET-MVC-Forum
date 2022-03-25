﻿namespace ASP.NET_MVC_Forum.Web.Infrastructure.Extensions
{
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Web.Services.Business.Category;
    using ASP.NET_MVC_Forum.Web.Services.Business.Censor;
    using ASP.NET_MVC_Forum.Web.Services.Business.Chat;
    using ASP.NET_MVC_Forum.Web.Services.Business.Comment;
    using ASP.NET_MVC_Forum.Web.Services.Business.CommentReport;
    using ASP.NET_MVC_Forum.Web.Services.Business.EmailSender;
    using ASP.NET_MVC_Forum.Web.Services.Business.HtmlManipulator;
    using ASP.NET_MVC_Forum.Web.Services.Business.Post;
    using ASP.NET_MVC_Forum.Web.Services.Business.PostReport;
    using ASP.NET_MVC_Forum.Web.Services.Business.User;
    using ASP.NET_MVC_Forum.Web.Services.Business.UserAvatar;
    using ASP.NET_MVC_Forum.Web.Services.Business.Vote;
    using ASP.NET_MVC_Forum.Web.Services.Data.Category;
    using ASP.NET_MVC_Forum.Web.Services.Data.Chart;
    using ASP.NET_MVC_Forum.Web.Services.Data.Chat;
    using ASP.NET_MVC_Forum.Web.Services.Data.Comment;
    using ASP.NET_MVC_Forum.Web.Services.Data.CommentReport;
    using ASP.NET_MVC_Forum.Web.Services.Data.Post;
    using ASP.NET_MVC_Forum.Web.Services.Data.PostReport;
    using ASP.NET_MVC_Forum.Web.Services.Data.User;
    using ASP.NET_MVC_Forum.Web.Services.Data.Vote;

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
            services.AddTransient<IUserDataService, UserDataService>();
            services.AddTransient<ICategoryDataService, CategoryDataService>();
            services.AddTransient<IPostBusinessService, PostBusinessService>();
            services.AddTransient<IPostDataService, PostDataService>();
            services.AddTransient<ICommentDataService, CommentDataService>();
            services.AddTransient<IPostReportDataService, PostReportDataService>();
            services.AddTransient<IPostReportBusinessService, PostReportBusinessService>();
            services.AddTransient<ICensorService, CensorService>();
            services.AddTransient<ICommentReportDataService, CommentReportDataService>();
            services.AddTransient<IProfanityFilter, ProfanityFilter>();
            services.AddTransient<IVoteBusinessService, VoteBusinessService>();
            services.AddTransient<IVoteDataService, VoteDataService>();
            services.AddTransient<IUserAvatarService, UserAvatarService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IChartDataService, ChartDataService>();
            services.AddTransient<IHtmlSanitizer>(s => new HtmlSanitizer());
            services.AddTransient<IChatDataService, ChatDataService>();
            services.AddTransient<IHtmlManipulator, HtmlManipulator>();
            services.AddTransient<IUserBusinessService, UserBusinessService>();
            services.AddTransient<IChatBusinessService, ChatBusinessService>();
            services.AddTransient<ICategoryBusinessService, CategoryBusinessService>();
            services.AddTransient<ICommentReportBusinessService, CommentReportBusinessService>();
            services.AddTransient<ICommentBusinessService, CommentBusinessService>();
            services.AddSingleton(configuration);
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
            });
            services.AddMemoryCache();
        }

        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
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
