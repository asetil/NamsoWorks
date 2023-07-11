using Aware.Util.Enum;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArzTalep.Web.Helper
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, string schemaName)
        {
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = schemaName;
                o.DefaultScheme = schemaName;
                o.DefaultForbidScheme = schemaName;
            })
            .AddScheme<AuthenticateCustomerHandlerOptions, AuthenticateCustomerHandler>(schemaName, null);

            return services;
        }

        public static IServiceCollection AddCookieAuthentication(this IServiceCollection services, string schemaName)
        {
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
            {
                o.LoginPath = WebConstants.PageUrl.Login;
                o.Cookie.Name = "ArzTalepUser";
            });

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, string secretKey)
        {
            //https://www.codeproject.com/Articles/5160941/ASP-NET-CORE-Token-Authentication-and-Authorizatio

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(token =>
            {
                token.RequireHttpsMetadata = false;
                token.SaveToken = true;
                token.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    //Same Secret key will be used while creating the token
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    //Usually, this is your application base URL
                    ValidIssuer = "http://localhost:45092/",
                    ValidateAudience = true,
                    //Here, we are creating and using JWT within the same application.
                    //In this case, base URL is fine.
                    //If the JWT is created using a web service, then this would be the consumer URL.
                    ValidAudience = "http://localhost:45092/",
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }

        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services, string schemaName)
        {
            services.AddAuthorization(options =>
            {
                //options.DefaultPolicy = new AuthorizationPolicyBuilder()
                //    .RequireAuthenticatedUser()
                //    .RequireClaim(ClaimTypes.Role, ((int)CustomerRole.User).ToString()) 
                //    .Build();

                var roles = Enum.GetNames(typeof(CustomerRole));
                foreach (var item in roles)
                {
                    Enum.TryParse(item, out CustomerRole enumType);
                    options.AddPolicy(item, policy =>
                    {
                        policy.Requirements.Add(new CustomAuthorizeRequirement(enumType));
                        policy.AuthenticationSchemes = new List<string> { schemaName };
                        policy.RequireAuthenticatedUser();
                    });
                }
            });

            return services;
        }
    }
}
