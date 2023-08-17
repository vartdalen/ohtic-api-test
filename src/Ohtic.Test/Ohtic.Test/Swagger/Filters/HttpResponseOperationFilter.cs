using Microsoft.OpenApi.Models;
using Ohtic.Test.Products.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;

namespace Ohtic.Test.Products.Swagger.Filters
{
	internal class HttpResponseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Remove the default 200 response
            var responsesToRemove = operation.Responses.Where(r => r.Key == StatusCodes.Status200OK.ToString()).ToList();
            foreach (var response in responsesToRemove)
            {
                operation.Responses.Remove(response.Key);
            }

            if (context.ApiDescription.HttpMethod == HttpMethod.Post.Method)
            {
                operation.Responses.Add(HttpStatusCode.Created);
                operation.Responses.Add(HttpStatusCode.BadRequest);
                operation.Responses.Add(HttpStatusCode.Conflict);
                operation.Responses.Add(HttpStatusCode.NotFound);
                operation.Responses.Add(HttpStatusCode.UnprocessableEntity);
            }

            if (context.ApiDescription.HttpMethod == HttpMethod.Get.Method)
            {
                operation.Responses.Add(HttpStatusCode.OK);
                operation.Responses.Add(HttpStatusCode.BadRequest);
                operation.Responses.Add(HttpStatusCode.NotFound);
            }

            if (context.ApiDescription.HttpMethod == HttpMethod.Patch.Method)
            {
                operation.Responses.Add(HttpStatusCode.NoContent);
                operation.Responses.Add(HttpStatusCode.BadRequest);
                operation.Responses.Add(HttpStatusCode.Conflict);
                operation.Responses.Add(HttpStatusCode.NotFound);
                operation.Responses.Add(HttpStatusCode.UnprocessableEntity);
            }

            if (context.ApiDescription.HttpMethod == HttpMethod.Delete.Method)
            {
                operation.Responses.Add(HttpStatusCode.NoContent);
                operation.Responses.Add(HttpStatusCode.NotFound);
            }
        }
    }
}
