using Microsoft.AspNetCore.Authorization;
using Ohtic.Test.Products.Middleware;

namespace Ohtic.Test.Products.Extensions
{
	internal static class WebApplicationExtensions
    {
        internal static void Configure(this WebApplication app)
        {
			app
                .UseCors()
                .UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ohtic.Test"); })
                .UseRouting()
                .UseAuthentication()
                .UseMiddleware<AuthenticatedMiddleware>()
                .UseMiddleware<ForbiddenMiddleware>()
                .UseMiddleware<UnauthorizedMiddleware>()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapSwagger()
                        .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Cookie,Bearer", Roles = "admin" });
                });

            app.MapControllers();
        }
    }
}