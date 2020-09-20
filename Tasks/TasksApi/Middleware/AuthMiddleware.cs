using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TasksApi.Service;

namespace TasksApi.Middleware
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public int ExpirationInDays { get; set; }
    }

    public static class AuthenticationExtension
    {
        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services, IConfiguration config)
        {

            var secret = config.GetSection("AppSettings").GetSection("Secret").Value;
            var key = Encoding.UTF8.GetBytes(secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };

                options.SecurityTokenValidators.Clear();
                options.SecurityTokenValidators.Add(new RevokableJwtSecurityTokenHandler(services.BuildServiceProvider()));
            });

            return services;
        }
    }
}

