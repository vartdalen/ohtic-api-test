using Microsoft.AspNetCore.Authorization;
using Ohtic.Test.Products.Authorization.Requirements;

namespace Ohtic.Test.Products.Authorization.Handlers
{
	public class AdminOrCustomerIdRouteValueMatchAuthorizationHandler :
        AuthorizationHandler<AdminOrCustomerIdRouteValueMatchRequirement>
    {
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AdminOrCustomerIdRouteValueMatchRequirement requirement
        )
        {
            if (context.User.IsInRole("admin") is true || IsCustomerIdRouteValueMatch(context))
                context.Succeed(requirement);
            else context.Fail();
            await Task.CompletedTask;
        }

        private static bool IsCustomerIdRouteValueMatch(AuthorizationHandlerContext context)
        {
            var httpContext = context.Resource as HttpContext;
            var claim = context?.User?.FindFirst(x => x.Type == "customer_id")?.Value;
            var routeValue = httpContext?.Request?.RouteValues["id"] as string;
            return claim == routeValue;
        }
    }
}