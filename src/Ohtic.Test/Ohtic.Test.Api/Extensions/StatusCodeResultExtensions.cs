using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Ohtic.Test.Api.Extensions
{
    internal static class StatusCodeResultExtensions
    {
        internal static ObjectResult WithDetail(this StatusCodeResult result, string? message)
        {
            var problemDetails = new ProblemDetails
            {
                Type = $"https://httpstatuses.com/{result.StatusCode}",
                Title = Enum
                    .GetName(typeof(HttpStatusCode), result.StatusCode)?
                    .FromPascalCaseToPascalCaseWithSpaces(),
                Status = result.StatusCode,
                Detail = message
            };

            return new ObjectResult(problemDetails)
            {
                StatusCode = result.StatusCode
            };
        }

        internal static ObjectResult WithInvalidIdentifier(this NotFoundResult result, string identifier)
        {
            var problemDetails = new ProblemDetails
            {
                Type = "https://www.rfc-editor.org/rfc/rfc9110.html#name-404-not-found",
                Title = Enum
                    .GetName(typeof(HttpStatusCode), result.StatusCode)?
                    .FromPascalCaseToPascalCaseWithSpaces(),
                Status = result.StatusCode,
                Detail = $"Invalid {nameof(identifier)} '{identifier}'"
            };

            return new ObjectResult(problemDetails)
            {
                StatusCode = result.StatusCode
            };
        }
    }
}