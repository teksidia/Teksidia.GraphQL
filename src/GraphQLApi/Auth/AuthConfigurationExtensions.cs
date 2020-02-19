using System;
using System.Collections.Generic;
using GraphQL.Authorization;
using GraphQL.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GraphQLApi.Auth
{
    public static class AuthConfigurationExtensions
    {
        public static void AddJwtAuthentication(this IServiceCollection services, IConfigurationSection config)
        {
            services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = $"{config["Instance"]}/{config["Domain"]}";
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidAudiences = new List<string> { config["ClientId"], $"api://{ config["ClientId"] }" },
                        ValidIssuers = new List<string> { $"{config["Instance"]}/{config["TenantId"]}/v2.0" }
                    };
                    options.RequireHttpsMetadata = false;
                });
        }

        public static void AddGraphQLAuthPolicies(this IServiceCollection services, string whiteListCsv)
        {
            var whiteList = whiteListCsv?.Split(',');
            services.AddGraphQLAuth((_, s) =>
            {
                _.AddPolicy("IsAuthenticatedPolicy", p => p.RequireAuthenticatedUser());
                _.AddPolicy("SensitiveDataPolicy", p => p.AddRequirement(new SensitiveDataRequirement(whiteList)));
            });
        }

        private static void AddGraphQLAuth(this IServiceCollection services, Action<AuthorizationSettings, IServiceProvider> configure)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IAuthorizationEvaluator, AuthorizationEvaluator>();
            services.AddTransient<IValidationRule, AuthValidationRule>();

            services.TryAddTransient(s =>
            {
                var authSettings = new AuthorizationSettings();
                configure(authSettings, s);
                return authSettings;
            });
        }
    }
}
