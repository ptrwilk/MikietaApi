using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Jwt.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection collection)
        {
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT authentication for MinimalAPI"
            };

            var securityRequirements = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[]{}
                }
            };

            var info = new OpenApiInfo
            {
                Version = "V1",
                Title = "Api with JWT Authentication",
                Description = "Api with JWT Authentication",
            };

            collection.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", info);
                options.AddSecurityDefinition("Bearer", securityScheme);
                options.AddSecurityRequirement(securityRequirements);
            });

            return collection;
        }

        public static IServiceCollection AddAuthenticationWithJwt(this IServiceCollection collection, string key)
        {
            collection.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                };
            });

            return collection;
        }
    }
}
