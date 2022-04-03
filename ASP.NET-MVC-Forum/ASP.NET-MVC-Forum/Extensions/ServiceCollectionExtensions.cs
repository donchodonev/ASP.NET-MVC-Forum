﻿namespace ASP.NET_MVC_Forum.Web.Infrastructure.Extensions
{
    using ASP.NET_MVC_Forum.Business;
    using ASP.NET_MVC_Forum.Business.Contracts;
    using ASP.NET_MVC_Forum.Business.Contracts.Contracts;
    using ASP.NET_MVC_Forum.Data;
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Infrastructure;
    using ASP.NET_MVC_Forum.Infrastructure.MappingProfiles;

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

            services.AddTransient<ICategoryRepository, CategoryRepository>();

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

            services.AddTransient<IChartRepository, ChartRespository>();

            services.AddTransient<IHtmlSanitizer>(s => new HtmlSanitizer());

            services.AddTransient<IChatRepository, ChatRepository>();

            services.AddTransient<IHtmlManipulator, HtmlManipulator>();

            services.AddTransient<IUserBusinessService, UserBusinessService>();

            services.AddTransient<IChatBusinessService, ChatBusinessService>();

            services.AddTransient<ICommentReportBusinessService, CommentReportBusinessService>();

            services.AddTransient<ICommentBusinessService, CommentBusinessService>();

            services.AddTransient<IChartService, ChartService>();

            services.AddSingleton(configuration);

            services.AddAutoMapper(
                typeof(CategoryMappingProfile).Assembly,
                typeof(CommentMappingProfile).Assembly,
                typeof(CommentReportMappingProfile).Assembly,
                typeof(MessageMappingProfile).Assembly,
                typeof(PostMappingProfile).Assembly,
                typeof(PostReportMappingProfile).Assembly,
                typeof(UserMappingProfile).Assembly,
                typeof(VoteMappingProfile).Assembly);

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