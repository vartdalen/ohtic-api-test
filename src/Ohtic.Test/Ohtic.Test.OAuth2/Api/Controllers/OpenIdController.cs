using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Ohtic.Test.OAuth.Api.Controllers
{
    [ApiController]
    public class OpenIdController : ControllerBase
    {
        private readonly IConfiguration _config;

        public OpenIdController(IConfiguration config)
        {
            _config = config;
        }

        [Authorize(AuthenticationSchemes = "Cookie,Bearer")]
        [HttpGet("/userinfo")]
        public IActionResult GetUserInfo()
        {
            var claims = User.Claims
                .OrderBy(c => c.Type)
                .ToDictionary(c => c.Type, c => c.Value);
            return Ok(claims);
        }

        [HttpGet("/.well-known/openid-configuration")]
        public IActionResult GetOpenIdConfiguration()
        {
            var metadata = new
            {
                issuer = _config["AppSettings:OAuth:Ohtic:Issuer"],
                token_endpoint = _config["AppSettings:OAuth:TokenUri"],
                scopes_supported = new string[] { "openid", "email", "profile" },
                response_types_supported = new string[] { "token id_token", "token" },
                id_token_signing_alg_values_supported = new string[] { "HS256" },
                http_logout_supported = true,
                end_session_endpoint = _config["AppSettings:OAuth:LogoutUri"],
                userinfo_endpoint = _config["AppSettings:OAuth:UserinfoUri"],
                claims_supported = new string[] { "aud", "azp", "email", "exp", "family_name", "given_name", "hd", "iat", "iss", "role", "sub" }
            };

            return Ok(metadata);
        }
    }
}
