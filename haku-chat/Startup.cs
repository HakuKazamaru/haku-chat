using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NLog;
using NLog.Web;
using haku_chat.Common.DataBase.Store;
using haku_chat.Common.Utility;
using haku_chat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using haku_chat.DbContexts;
using NuGet.Protocol;

namespace haku_chat
{
    public class Startup
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static Logger logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            logger.Info("========== Func Start! ==================================================");

            // 分散キャッシュの指定(アプリのインスタンス内で有効)
            services.AddDistributedMemoryCache();
            // クッキーポリシーの設定
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddHttpContextAccessor();

            // セッションサービスの追加
            services.AddSession(opt =>
            {
                // オプション指定
                opt.IdleTimeout = TimeSpan.FromMinutes(30);
                opt.Cookie.IsEssential = true;
                opt.Cookie.MaxAge = TimeSpan.FromMinutes(30);
            });
            // DBContextの設定
            services.AddDbContext<ChatDbContext>(options =>
            {
                string dbType = Config.GetAppsettingsToSectionStringValue("DBType");
                // ログ出力設定
                options.EnableSensitiveDataLogging();

                // DB接続先指定
                switch (dbType)
                {
                    case "mysql":
                        {
                            logger.Info("MySQLモードで実行します。");
                            options.UseMySQL(Configuration.GetConnectionString("Context"), providerOptions =>
                            {

                            });
                            break;
                        }
                    case "pgsql":
                        {
                            logger.Info("PostgerSQLモードで実行します。");
                            options.UseNpgsql(Configuration.GetConnectionString("Context"), providerOptions =>
                            {
                                providerOptions.EnableRetryOnFailure();
                            });
                            break;
                        }
                    case "mssql":
                        {
                            logger.Info("MicrosoftSQLモードで実行します。");
                            options.UseSqlServer(Configuration.GetConnectionString("Context"), providerOptions =>
                            {
                                providerOptions.EnableRetryOnFailure();
                            });
                            break;
                        }
                    default:
                        {
                            logger.Warn("DBTypeに不正な値が指定されています。設定値：{0}", dbType);
                            goto case "mysql";
                        }
                }
            });

            services.AddDatabaseDeveloperPageExceptionFilter();

            // ユーザー管理用Identityモデルの設定
            services.AddDefaultIdentity<UserMasterModel>()
                .AddUserStore<UserMasterStore>()
                .AddEntityFrameworkStores<ChatDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<UserMasterStore>();

            // Identityの設定
            services.Configure<IdentityOptions>(options =>
            {
                // 認証情報関連
                options.ClaimsIdentity.UserIdClaimType = "Id";

                // パスワードポリシー
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // ロックアウトポリシー
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // ユーザー情報設定
                options.User.AllowedUserNameCharacters = null; // ユーザー名に使用可能な文字制限を解除
                options.User.RequireUniqueEmail = true; // e-mailを必須にしない

                // サインイン要求要素
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedAccount = false;
            });

            // ログイン関連の設定
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.Name = "Haku'sChatSystem";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.LoginPath = new PathString("/Auth/LogIn");
                options.LogoutPath = new PathString("/Auth/LogOut");
                options.AccessDeniedPath = new PathString("/Auth/AccessDenied"); //AccessDenied は未実装
                options.SlidingExpiration = true;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                // options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            }).AddCookie(cookieOptions =>
            {
                cookieOptions.LoginPath = "/Auth/LogIn";
                cookieOptions.LogoutPath = "/Auth/LogOut";
            }).AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            })
            /*
            .AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration["Authentication:FB:AppId"];
                facebookOptions.AppSecret = Configuration["Authentication:FB:AppSecret"];
                facebookOptions.ClientId= Configuration["Authentication:FB:ClientId"];
                facebookOptions.ClientSecret = Configuration["Authentication:FB:ClientSecret"];
            })
            .AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = Configuration["Authentication:Microsoft:ClientId"];
                microsoftOptions.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];
            }).AddTwitter(twitterOptions =>
            {
                twitterOptions.ConsumerKey = Configuration["Authentication:Twitter:ConsumerAPIKey"];
                twitterOptions.ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];
                twitterOptions.RetrieveUserDetails = true;
            })
            */
            ;

            services.AddAuthorization(options =>
            {
                // AllowAnonymous 属性が指定されていないすべての Action などに対してユーザー認証が必要となる
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.AddMvc();

            logger.Info("========== Func End!   ==================================================");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            logger.Info("========== Func Start! ==================================================");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            // クライアントのIPアドレスを取得するため、NginxのProxyヘッダを使う設定
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseHttpMethodOverride();

            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            // セッションを使用
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Lobby}/{action=Index}/{id?}");
            });
            logger.Info("========== Func End!   ==================================================");
        }
    }
}
