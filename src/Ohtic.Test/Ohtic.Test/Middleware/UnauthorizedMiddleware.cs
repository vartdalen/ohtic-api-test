using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Ohtic.Test.Products.Factories;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Ohtic.Test.Products.Middleware
{
	internal sealed class UnauthorizedMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        private readonly JsonSerializerOptions _jsonOptions;

        public UnauthorizedMiddleware(
            RequestDelegate next,
            IConfiguration config,
            JsonSerializerOptions jsonOptions
        )
        {
            _next = next;
            _config = config;
            _jsonOptions = jsonOptions;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized && !context.Response.HasStarted)
            {
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(ProblemDetails(context), _jsonOptions));
            }
        }

        private ProblemDetails ProblemDetails(HttpContext context)
        {
            var problemDetails = ProblemDetailsFactory.Create(
                context.Request,
                HttpStatusCode.Unauthorized,
                HttpStatusCode.Unauthorized.ToString(),
				"Please follow one of the 'auth_urls' to authenticate with Ohtic.Test - choose 'cookie_session_url' for 'Cookie' scheme (browser), 'tokenUrl' for 'Bearer' scheme (access_token)"
			);
            problemDetails.Extensions.Add("auth_urls", new List<object>
            {
                new
                {
                    identity_provider = "Google",
                    cookie_session_url = GoogleUri(context, true),
                    token_url = GoogleUri(context, false)
                },
                new
                {
                    identity_provider = "Microsoft",
					cookie_session_url = MicrosoftUri(context, true),
                    token_url = MicrosoftUri(context, false)
                }
            });
            return problemDetails;
        }

        private Uri GoogleUri(HttpContext context, bool is_cookie_session_enabled)
        {
            return new Uri(
                $"{_config["AppSettings:OAuth:Google:Issuer"]}/o/oauth2/v2/auth?" +
                $"client_id={_config["AppSettings:OAuth:Google:Web:ClientId"]}&" +
                $"response_type=code" +
                $"&redirect_uri={_config["AppSettings:OAuth:Google:Web:RedirectUri"]}&" +
                $"scope=openid%20profile%20email&" +
                $"state={Convert.ToBase64String(EncodeState(State("google", is_cookie_session_enabled, context.Request.GetEncodedPathAndQuery())))}"
            );
        }

        private Uri MicrosoftUri(HttpContext context, bool is_cookie_session_enabled)
        {
            return new Uri(
                $"{_config["AppSettings:OAuth:Microsoft:TokenBaseUri"]}/authorize?" +
                $"client_id={_config["AppSettings:OAuth:Microsoft:Web:ClientId"]}&" +
                $"response_type=code" +
                $"&redirect_uri={_config["AppSettings:OAuth:Microsoft:Web:RedirectUri"]}&" +
                $"scope=openid%20profile%20email&" +
                $"state={Convert.ToBase64String(EncodeState(State("microsoft", is_cookie_session_enabled, context.Request.GetEncodedPathAndQuery())))}"
            );
        }

        private static Dictionary<string, string> State(
            string identity_provider,
            bool is_cookie_session_enabled,
            string redirect_path
        )
        {
            return new()
            {
                { "identity_provider", identity_provider },
                { "is_cookie_session_enabled", is_cookie_session_enabled.ToString() },
                { "redirect_path", redirect_path}
            };
        }

        private static byte[] EncodeState(Dictionary<string, string> state)
        {
            var json = JsonSerializer.Serialize(state);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}