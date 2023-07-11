using System;
using System.Net;
using System.Threading.Tasks;
using Aware.Dependency;
using Aware.Util;
using Aware.Util.Enum;
using Cinescope.Data;
using Cinescope.Web.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cinescope.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var dependencySetting = new DependencySetting()
            {
                OrmType = Configuration.GetEnum<ORMType>("OrmType"),
                ConnectionString = Configuration.GetConnectionString("CinescopeCS"),
                DbType = Configuration.GetEnum<DatabaseType>("DbType"),
                CacheMode = Configuration.GetEnum<CacheMode>("CacheMode"),
                UseIntercepter = Configuration.GetValue<bool>("UseIntercepter"),
                IsLite = true
            };
            var dependencyInstaller = new DependencyResolver();
            dependencyInstaller.Install(ref services, dependencySetting);

            if (dependencySetting.OrmType == ORMType.EntityFramework)
            {
                services.AddDbContext<CinescopeDbContext>(opt => opt.UseSqlServer(dependencySetting.ConnectionString));
            }

            services.AddScoped<IServiceProvider, ServiceProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            //services.AddScoped<ISessionHelper, SessionHelper>();

            //services.AddSingleton<IAuthorizationRequirement, CustomAuthorizeRequirement>();
            //services.AddSingleton<IAuthorizationHandler, CustomAuthorizationHandler>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //services
            //    .AddCustomAuthentication(Constants.AuthenticationSchema)
            //    .AddCustomAuthentication(Constants.AuthenticationSchema);

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.IsEssential = true;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();

            app.UseAuthentication();

            //app.UseStatusCodePages(context =>
            //{
            //    var response = context.HttpContext.Response;
            //    if (response.StatusCode == (int)HttpStatusCode.Unauthorized ||
            //        response.StatusCode == (int)HttpStatusCode.Forbidden)
            //        response.Redirect(Constants.PageUrl.Login + "?returnUrl=" + context.HttpContext.Request.Path);
            //    if (response.StatusCode == (int)HttpStatusCode.NotFound)
            //        response.Redirect("/sayfa-bulunamadi");
            //    else if (response.StatusCode >= (int)HttpStatusCode.BadRequest) //400 ustu hata kodlari icin
            //        response.Redirect("/error/" + response.StatusCode);
            //    return Task.CompletedTask;
            //});

            app.UseMvc(RouteHelper.MapRoutes);

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});
        }
    }
}
