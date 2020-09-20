using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TasksApi.Exceptions;

namespace TasksApi.Service
{
    public class RevokableJwtSecurityTokenHandler : JwtSecurityTokenHandler
    {
        private static IDistributedCache _cache;

        public RevokableJwtSecurityTokenHandler(ServiceProvider serviceProvider)
        {
            _cache = serviceProvider.GetRequiredService<IDistributedCache>();
        }

        public override ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            var claimsPrincipal = base.ValidateToken(securityToken, validationParameters, out validatedToken);
            // Get JWT ID
            var claim = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Jti);
            // Check if JWT is blacklisted
            if (claim != null && claim.ValueType == ClaimValueTypes.String)
                if (_cache.Get(claim.Value) != null)
                    throw new SecurityTokenRevokedException($"The token has been revoked, securitytoken: '{validatedToken}'.");
         
            return claimsPrincipal;
        }
    }
}
