using Microsoft.AspNetCore.Mvc;
using Ohtic.Test.Products.Factories;
using System.Net;
using System.Text.Json;

namespace Ohtic.Test.Products.Middleware
{
	internal sealed class ForbiddenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerOptions _jsonOptions;

        public ForbiddenMiddleware(
            RequestDelegate next,
            JsonSerializerOptions jsonOptions
        )
        {
            _next = next;
            _jsonOptions = jsonOptions;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(ProblemDetails(context), _jsonOptions));
            }
        }

        private static ProblemDetails ProblemDetails(HttpContext context)
        {
            var problemDetails = ProblemDetailsFactory.Create(
                context.Request,
                HttpStatusCode.Forbidden,
                HttpStatusCode.Forbidden.ToString(),
                "You do not have permission to access this resource"
            );
            if (context.Response.Headers["X-Logout-Url"].Any()) {
                problemDetails.Extensions.Add("logout_url", (string?)context.Response.Headers["X-Logout-Url"]);
            }
            return problemDetails;
        }
    }
}