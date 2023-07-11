using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Worchart.API.Helper;
using Worchart.API.Middleware;
using Worchart.BL;
using Worchart.BL.Constants;
using Worchart.BL.Dependency;
using Worchart.BL.Enum;

namespace Worchart.API
{
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
            ArrangeSwaggerDoc(ref services);

            var dependencySetting = GetDependencySettings();
            var libraryInstaller = new LibraryInstaller();
            libraryInstaller.Install(ref services, dependencySetting);

            //Add DB Context
            //services.AddDbContext<TripContext>(options => options.UseSqlite("Data Source=JeffsTrips.db"));

            //services.AddDistributedRedisCache(options =>
            //{
            //    options.Configuration = Configuration.GetValue("RedisServer");
            //    options.ConfigurationOptions.ConnectTimeout = 10000;
            //});

            services.AddSingleton<IAuthorizationRequirement, AuthorizeCustomerRequirement>();
            services.AddSingleton<IAuthorizationHandler, AuthorizeCustomerHandler>();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(opt =>
                {
                    opt.UseCamelCasing(true);
                    opt.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });
            //.AddFluentValidation(cfg =>
            //{
            //    cfg.RegisterValidatorsFromAssemblyContaining<Startup>();
            //});

            //var corsBuilder = new CorsPolicyBuilder();
            //corsBuilder.AllowAnyHeader();
            //corsBuilder.AllowAnyMethod();
            //corsBuilder.AllowAnyOrigin(); // For anyone access.
            ////corsBuilder.WithOrigins("http://localhost:56573"); // for a specific url. Don't add a forward slash on the end!
            //corsBuilder.AllowCredentials();

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("SiteCorsPolicy", corsBuilder.Build());
            //});
            services.AddCors();

            services
                .AddCustomAuthentication()
                .AddCustomAuthorization();
            //services.AddJWT();

            //services.AddRouting(options => options.LowercaseUrls = true);
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowAllOrigins",
            //        builder =>
            //        {
            //            builder
            //                .AllowAnyOrigin()
            //                .AllowAnyHeader()
            //                .AllowAnyMethod();
            //        });
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //app.UseCors("SiteCorsPolicy");
            app.UseCors(o => o.WithOrigins(Configuration.GetValue("CorsOrigin"))
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials());

            //app.Use(async (context, next) =>
            //{
            //    var sw = new Stopwatch();
            //    sw.Start();
            //    await next.Invoke();
            //    sw.Stop();

            //    context.Response.Headers.Add("nms_duration", sw.ElapsedMilliseconds.ToString());
            //});

            //app.UseNmsMiddleware();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();

            if (env.IsDevelopment() || 1 == 1)
            {
                app.UseSwagger().UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/doc/swagger.json", "Worchart.Api");
                });
            }

            //app.UseMiddleware<TokenCheckMiddleware>();

            //Custom middleware : Add JWToken to all incoming HTTP Request Header
            //app.Use(async (context, next) =>
            //{
            //    var JWToken = context.Session.GetString("JWToken");
            //    if (!string.IsNullOrEmpty(JWToken))
            //    {
            //        context.Request.Headers.Add("Authorization", "Bearer " + JWToken);
            //    }
            //    await next();
            //});

            app.UseHttpsRedirection();

            //app.UseAuthorization();

            app.UseMvc();

            //To get a service from DI
            //using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            //{
            //    var context = serviceScope.ServiceProvider.GetService<TripContext>();
            //}
        }

        private DependencySetting GetDependencySettings()
        {
            var dependencySetting = new DependencySetting()
            {
                OrmType = Configuration.GetEnum<ORMType>("OrmType"),
                ConnectionString = Configuration.GetConnectionString("WorchartCS"),
                DbType = Configuration.GetEnum<DatabaseType>("DbType"),
                CacheMode = Configuration.GetEnum<CacheMode>("CacheMode"),
                UseIntercepter = Configuration.GetValue<bool>("UseIntercepter")
            };
            return dependencySetting;
        }

        private void ArrangeSwaggerDoc(ref IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("doc", new Info
                {
                    Title = "Worchart Web Api",
                    Version = "1.0.0",
                    Description = "Çalışma Sanatı",
                    Contact = new Contact()
                    {
                        Name = "worchart.com.tr",
                        Url = "http://www.worchart.com.tr",
                        Email = "support@worchart.com.tr"
                    },
                    TermsOfService = "http://swagger.io/terms/",
                });

                //options.DescribeAllEnumsAsStrings();

                //For JWT Tokens
                //options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                //{
                //    Name = "Authorization",
                //    Type = SecuritySchemeType.ApiKey,
                //    Scheme = "Bearer",
                //    BearerFormat = "JWT",
                //    In = ParameterLocation.Header,
                //    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                //});

                //swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //          new OpenApiSecurityScheme
                //            {
                //                Reference = new OpenApiReference
                //                {
                //                    Type = ReferenceType.SecurityScheme,
                //                    Id = "Bearer"
                //                }
                //            },
                //            new string[] {}

                //    }
                //});

                options.AddSecurityDefinition("oauth2", new ApiKeyScheme
                {
                    Description = "API request customer user token taken from login process",
                    In = "header",
                    Name = CommonConstants.CustomerUserToken,
                    Type = "apiKey"
                });

                //x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    In = ParameterLocation.Header,
                //    Description = "Please insert JWT with Bearer into field",
                //    Name = "Authorization",
                //    Type = SecuritySchemeType.ApiKey,
                //    BearerFormat = "JWT"
                //});

                //options.AddSecurityDefinition("bearer", new ApiKeyScheme
                //{
                //    Description = "API request customer user token taken from login process",
                //    In = "header",
                //    Name = CommonConstants.CustomerUserToken,
                //    Type = "apiKey"
                //});

                options.OperationFilter<AddHeaderOperationFilter>(CommonConstants.AccessToken, "API request access token info", false);


                var aa = new Dictionary<string, IEnumerable<string>>
                {
                    { CommonConstants.AccessToken, new List<string>() },
                    { CommonConstants.CustomerUserToken, new List<string>() }
                };
                options.AddSecurityRequirement(aa);
                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });
        }
    }
}
