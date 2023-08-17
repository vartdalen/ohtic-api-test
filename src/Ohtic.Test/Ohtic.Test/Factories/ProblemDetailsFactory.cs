using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Ohtic.Test.Products.Factories
{
    internal static class ProblemDetailsFactory
    {
        internal static ProblemDetails Create(
            HttpRequest request,
            HttpStatusCode statusCode,
            string title,
            string detail
        )
        {
            return new ProblemDetails
            {
                Type = $"https://httpstatuses.com/{(int)statusCode}",
                Title = title,
                Status = (int)statusCode,
                Detail = detail,
                Instance = $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}"
            };
        }
    }
}