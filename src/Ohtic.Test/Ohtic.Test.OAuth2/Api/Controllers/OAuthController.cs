
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Ohtic.Test.Data.Models.Indexes;
using Ohtic.Test.OAuth.Api.Models;
using Ohtic.Test.OAuth.Api.Models.Abstractions;
using Ohtic.Test.OAuth.Api.Refit;
using Ohtic.Test.OAuth.Extensions;
using Ohtic.Test.OAuth.Services.Abstractions;
using Ohtic.Test.Services.Abstractions;
using Ohtic.Test.Services.Models.Dtos.Customers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Ohtic.Test.OAuth.Api.Controllers
{
	[Authorize(AuthenticationSchemes = "Cookie")]
    [ApiController]
    [Route("[controller]")]
    public class OAuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IGoogleOAuthApi _googleApi;
        private readonly IMicrosoftOAuthApi _microsoftApi;
        private readonly ICustomerService _customerService;
        private readonly ITokenService _tokenService;

        public OAuthController(
            IConfiguration config,
            IGoogleOAuthApi googleApi,
            IMicrosoftOAuthApi microsoftApi,
            ICustomerService customerService,
            ITokenService tokenService
        )
        {
            _config = config;
            _googleApi = googleApi;
            _microsoftApi = microsoftApi;
            _customerService = customerService;
            _tokenService = tokenService;
        }

		[AllowAnonymous]
		[HttpPost("token")]
		public async Task<IActionResult> Token([FromForm] TokenRequest request)
		{
			if (request.grant_type == "authorization_code") return await AuthorizationCode(request);
			else if (request.grant_type == "refresh_token") return await RefreshToken(request);
			return BadRequest($"Unsupported grant_type {request.grant_type}");
		}

		private async Task<IActionResult> RefreshToken([FromBody] TokenRequest request)
		{
			if (request.refresh_token == null) return BadRequest();
            try { await _tokenService.ValidateJwt(request.refresh_token); } catch (SecurityTokenExpiredException) { return Unauthorized(); }
            
			var isRefreshTokenInStore = InMemoryTokenStore.TryIdentifyRefreshToken(request.refresh_token, out string? userId);
			if (!isRefreshTokenInStore) return NotFound();

            var exPrincipal = await _tokenService.ValidateJwt(HttpContext.Request.Headers["Authorization"]!, false);
            if (exPrincipal.FindFirst(p => p.Type == "customer_id")?.Value != userId) return Unauthorized();

            exPrincipal.AddClaimsToDeepCopy(_config, out var atUserClaims);
			exPrincipal.AddClaimsToDeepCopy(_config, out var rtUserClaims, null, true);
            var accessToken = _tokenService.WriteJwt(atUserClaims, DateTimeOffset.UtcNow.AddMinutes(5));
			var refreshToken = _tokenService.WriteJwt(rtUserClaims, DateTimeOffset.UtcNow.AddMinutes(60));

			InMemoryTokenStore.RemoveRefreshToken(userId!);
			InMemoryTokenStore.AddRefreshToken(userId!, refreshToken);

			AddTokenResponseHeaders(accessToken, refreshToken);
			return Ok($"Token refresh successful, access_token and refresh_token granted, see tokens in response headers");
		}

		private async Task<IActionResult> AuthorizationCode(TokenRequest request)
		{
			var identityProvider = MapClientIdToIdentityProvider(request);
			var forwardedRequest = AuthorizationCodeTokenRequest(
				request.client_id!,
				request.code!,
				request.redirect_uri!,
				request.client_secret,
				request.code_verifier
			);
			if (identityProvider != null && identityProvider.IsGoogle()) return await AuthorizationCodeFlow(await _googleApi.Token(forwardedRequest));
			if (identityProvider != null && identityProvider.IsMicrosoft()) return await AuthorizationCodeFlow(await _microsoftApi.Token(forwardedRequest));
			return BadRequest($"No supported identity_provider for client_id {request.client_id}");
		}

		private string? MapClientIdToIdentityProvider(TokenRequest request)
		{
			var googleClientIds = new string[] { _config[$"AppSettings:OAuth:Google:Web:ClientId"]!, _config[$"AppSettings:OAuth:Google:Android:ClientId"]! };
			var microsoftClientIds = new string[] { _config[$"AppSettings:OAuth:Microsoft:Web:ClientId"]!, _config[$"AppSettings:OAuth:Microsoft:Android:ClientId"]! };
			return googleClientIds.Contains(request.client_id) ? "google" :
			    microsoftClientIds.Contains(request.client_id) ? "microsoft" :
			    null;
		}

		[AllowAnonymous]
        [HttpGet("login")]
        public async Task<IActionResult> Login(
            [FromQuery] string code,
            [FromQuery] string state
		)
        {
            if (HttpContext.User.Identity?.IsAuthenticated == true) {
                return BadRequest($"'{HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Email)?.Value}' is already authenticated");
            }
            if (!TryDecodeState(state, out var stateDict)) {
                return BadRequest($"Failed decoding {nameof(state)} '{state}'");
            }
            if (stateDict!.TryGetValue("identity_provider", out string? identity_provider))
            {
                if (identity_provider.IsGoogle())
                    return await AuthorizationCodeFlow(await _googleApi.Token(AuthorizationCodeTokenRequest(
					    _config[$"AppSettings:OAuth:Google:Web:ClientId"]!,
					    code,
					    _config[$"AppSettings:OAuth:Google:Web:RedirectUri"]!,
						_config[$"Secrets:OAuth:Google:Web:ClientSecret"]!
					)), stateDict);
                if (identity_provider.IsMicrosoft())
                    return await AuthorizationCodeFlow(await _microsoftApi.Token(AuthorizationCodeTokenRequest(
                        _config[$"AppSettings:OAuth:Microsoft:Web:ClientId"]!,
                        code,
					    _config[$"AppSettings:OAuth:Microsoft:Web:RedirectUri"]!,
						_config[$"Secrets:OAuth:Microsoft:Web:ClientSecret"]!
					)), stateDict);
            }
            return BadRequest($"{nameof(identity_provider)} '{identity_provider}' is not supported");
        }

        [AllowAnonymous]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            if (HttpContext.User.Identity?.IsAuthenticated == false) return Unauthorized();
            var email = HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Email)?.Value;
            await HttpContext.SignOutAsync("Cookie");
            return Ok($"'{email}' logged out");
        }

        private async Task<IActionResult> AuthorizationCodeFlow(
            ITokenResponse acTokenResponse,
            Dictionary<string, string>? state = null
        )
		{
			var acPrincipal = await _tokenService.ValidateJwt(acTokenResponse.IdToken);
			var email = acPrincipal.FindFirst(p => p.Type == ClaimTypes.Email)?.Value;
			if (string.IsNullOrEmpty(email)) { return BadRequest($"Registration failed, missing required claim: '{ClaimTypes.Email}'"); }

			ReadCustomerDto? readCustomerResult = null, createCustomerResult = null;
			try { readCustomerResult = await _customerService.Read(new CustomerEmail(email)); }
			catch (KeyNotFoundException) { createCustomerResult = await _customerService.Create(new CreateCustomerDto { Email = email }); }
			var id = readCustomerResult?.Id ?? createCustomerResult?.Id;
			if (id is null) return StatusCode(500);
			acPrincipal.AddClaimsToDeepCopy(_config, out var atClaims, id.ToString());
			acPrincipal.AddClaimsToDeepCopy(_config, out var rtClaims, null, true);
			var user_access_token = _tokenService.WriteJwt(atClaims, DateTimeOffset.UtcNow.AddMinutes(5));
			var user_refresh_token = _tokenService.WriteJwt(rtClaims, DateTimeOffset.UtcNow.AddMinutes(60));
			InMemoryTokenStore.AddRefreshToken(id.ToString()!, user_refresh_token);
			if (state != null && IsCookieSessionEnabled(state))
                return await LoginAndRedirect(state, await _tokenService.ValidateJwt(user_access_token));
			AddTokenResponseHeaders(user_access_token, user_refresh_token);
			return SuccessResult(createCustomerResult, email, id.ToString());
		}

		private async Task<IActionResult> LoginAndRedirect(
            Dictionary<string, string> state,
            ClaimsPrincipal userPrincipal
        )
		{
			state.TryGetValue("redirect_path", out string? redirect_path);
			if (redirect_path == null) return BadRequest($"Login failed, missing required state '{nameof(redirect_path)}'");
			await HttpContext.SignInAsync("Cookie", userPrincipal, AuthenticationProperties(userPrincipal.IsInRole("admin")));
			return LocalRedirect(redirect_path!);
		}

		private IActionResult SuccessResult(ReadCustomerDto? createCustomerResult, string? email, string? hashId)
		{
			return createCustomerResult switch
			{
				null => Ok($"Authentication successful, access_token and refresh_token granted for '{email}', see tokens in response headers"),
				not null => Created(
					$"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/customers/{hashId}",
					$"Authentication successful, account created, access_token granted for '{email}', see access_token in response headers")
			};
		}

		private static AuthenticationProperties AuthenticationProperties(bool isAdmin = false)
        {
            var authProperties = new AuthenticationProperties()
            {
                ExpiresUtc = isAdmin ? DateTime.UtcNow.AddMinutes(60) : DateTime.UtcNow.AddDays(30),
                IsPersistent = true,
                AllowRefresh = true
            };
            return authProperties;
        }

        private TokenRequest AuthorizationCodeTokenRequest(
            string clientId,
            string code,
            string redirectUri,
			string? clientSecret = null,
            string? codeVerifier = null

		)
        {
            return new TokenRequest
            {
				grant_type = "authorization_code",
				client_id = clientId,
				client_secret = clientSecret,
				code = code,
                redirect_uri = redirectUri,
                code_verifier = codeVerifier
            };
        }

        private static TokenRequest ClientCredentialsTokenRequest(string clientId, string clientSecret)
        {
            return new TokenRequest
            {
				grant_type = "client_credentials",
				client_id = clientId,
                client_secret = clientSecret,
                scope = $"api://{clientId}/.default"
            };
        }

        private static bool TryDecodeState(string base64, out Dictionary<string, string>? dict)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            string json = Encoding.UTF8.GetString(bytes);
            dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            return dict is not null;
        }

        private static bool IsCookieSessionEnabled(Dictionary<string, string> dict)
        {
            return dict.TryGetValue("is_cookie_session_enabled", out string? is_cookie_session_enabled) &&
                bool.TryParse(is_cookie_session_enabled, out bool parsedValue) &&
                parsedValue;
        }

        private void AddTokenResponseHeaders(string access_token, string refresh_token)
        {
            Response.Headers.Add("X-Access-Token", access_token);
			Response.Headers.Add("X-Refresh-Token", refresh_token);
        }

        private static bool TryGetTimeUntilExpiryInSeconds(string? expiryUnixSecondsString, out int timeUntilExpirySeconds)
        {
            if (long.TryParse(expiryUnixSecondsString, out long expiryUnixSeconds))
            {
                var currentUnixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var timeUntilExpirySecondsLong = expiryUnixSeconds - currentUnixSeconds;
                
                timeUntilExpirySeconds = (int)Math.Max(timeUntilExpirySecondsLong, 0);
                return true;
            }
            
            timeUntilExpirySeconds = 0;
            return false;
        }
    }
}