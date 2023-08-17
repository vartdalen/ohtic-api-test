using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Ohtic.Test.OAuth.Extensions
{
    internal static class ClaimsPrincipalExtensions
    {
        internal static void AddClaimsToDeepCopy(
            this ClaimsPrincipal principal,
            IConfiguration config,
            out List<Claim> claims,
            string? customerId = null,
			bool isRefreshToken = false
		)
        {
            if (isRefreshToken) claims = principal.Claims.Where(x => x.Type == "iss" || x.Type == "aud").ToList();
            else claims = principal.Claims.ToList();
			if (customerId is not null) claims.Add(new Claim("customer_id", customerId));
            if (!isRefreshToken && principal.IsGoogleAdmin(config)) claims.Add(new Claim(ClaimTypes.Role, "admin"));
        }

        private static bool IsGoogleAdmin(
            this ClaimsPrincipal principal,
            IConfiguration config
        )
        {
            var isIssuerClaimSet = principal.HasClaim(c => c.Type == "iss" && c.Value == config["AppSettings:OAuth:Google:Issuer"]);
            var isHdClaimSet = principal.HasClaim(c => c.Type == "hd" && c.Value == config["AppSettings:OAuth:Google:Admin:HostedDomain"]);
            if (isIssuerClaimSet && isHdClaimSet) return true;
            return false;
        }
    }
}