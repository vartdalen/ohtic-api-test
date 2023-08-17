using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Ohtic.Test.OAuth.Services.Abstractions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ohtic.Test.OAuth.Services
{
	public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<ClaimsPrincipal> ValidateJwt(string jwt, bool validateLifetime = true)
        {
            var googleSigningKeys = await SigningKeys(_config["AppSettings:OAuth:Google:ConfigurationUri"]);
            var microsoftSigningKeys = await SigningKeys(_config["AppSettings:OAuth:Microsoft:ConfigurationUri"]);
            var ohticKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Secrets:OAuth:Ohtic:SigningKey"]!));
            var signingKeys = googleSigningKeys
                .Concat(microsoftSigningKeys)
                .Append(ohticKey)
                .ToList();
            return new JwtSecurityTokenHandler().ValidateToken(jwt, TokenValidationParameters(signingKeys, validateLifetime), out _);
        }

        public string WriteJwt(IEnumerable<Claim> claims, DateTimeOffset expires)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Secrets:OAuth:Ohtic:SigningKey"]!));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var newToken = new JwtSecurityToken(
                issuer: _config["AppSettings:OAuth:Ohtic:Issuer"],
                claims: claims,
                expires: expires.UtcDateTime,
                signingCredentials: signingCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(newToken);
        }

        private static async Task<ICollection<SecurityKey>> SigningKeys(string? configurationUri)
        {
            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                configurationUri,
                new OpenIdConnectConfigurationRetriever()
            );
            var openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);
            var signingKeys = openIdConfig.SigningKeys;
            return signingKeys;
        }

        private TokenValidationParameters TokenValidationParameters(List<SecurityKey> signingKeys, bool validateLifetime)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuers = new string?[]
                {
                    _config["AppSettings:OAuth:Google:Issuer"],
                    $"{_config["AppSettings:OAuth:Microsoft:Issuer"]}/{_config["AppSettings:OAuth:Microsoft:TenantId"]}/v2.0",
                    _config["AppSettings:OAuth:Ohtic:Issuer"]
                },
                ValidateAudience = true,
                ValidAudiences = new string?[]
                {
                    _config["AppSettings:OAuth:Google:Web:ClientId"],
					_config["AppSettings:OAuth:Google:Android:ClientId"],
					_config["AppSettings:OAuth:Microsoft:Web:ClientId"],
                    _config["AppSettings:OAuth:Microsoft:Android:ClientId"],
				},
                IssuerSigningKeys = signingKeys,
                ValidateLifetime = validateLifetime
            };
        }
    }
}