using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using Worchart.API.Middleware;
using Worchart.BL.Constants;
using Worchart.BL.Enum;

namespace Worchart.API.Helper
{
    public static class AuthenticationHelper
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = CommonConstants.AuthenticationSchema;
                o.DefaultScheme = CommonConstants.AuthenticationSchema;
                o.DefaultForbidScheme = CommonConstants.AuthenticationSchema;
            })
            .AddScheme<AuthenticateCustomerHandlerOptions, AuthenticateCustomerHandler>(CommonConstants.AuthenticationSchema, null);

            return services;
        }

        public static IServiceCollection AddJWT(this IServiceCollection services, byte[] secretKey)
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
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
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

        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                var roles = Enum.GetNames(typeof(CustomerRole));
                foreach (var item in roles)
                {
                    Enum.TryParse(item, out CustomerRole enumType);
                    options.AddPolicy(item, policy =>
                    {
                        policy.Requirements.Add(new AuthorizeCustomerRequirement(enumType));
                        policy.AuthenticationSchemes = new List<string> { CommonConstants.AuthenticationSchema };
                        policy.RequireAuthenticatedUser();
                    });
                }
            });

            return services;
        }
    }
}
