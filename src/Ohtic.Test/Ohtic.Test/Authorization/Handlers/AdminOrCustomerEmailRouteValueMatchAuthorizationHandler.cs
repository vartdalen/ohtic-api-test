using Microsoft.AspNetCore.Authorization;
using Ohtic.Test.Products.Authorization.Requirements;
using System.Security.Claims;

namespace Ohtic.Test.Products.Authorization.Handlers
{
	public class AdminOrCustomerEmailRouteValueMatchAuthorizationHandler :
        AuthorizationHandler<AdminOrCustomerEmailRouteValueMatchRequirement>
    {
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AdminOrCustomerEmailRouteValueMatchRequirement requirement
        )
        {
            if (context.User.IsInRole("admin") is true || IsCustomerEmailRouteValueMatch(context))
                context.Succeed(requirement);
            else context.Fail();
            await Task.CompletedTask;
        }

        private static bool IsCustomerEmailRouteValueMatch(AuthorizationHandlerContext context)
        {
            var httpContext = context.Resource as HttpContext;
            var claim = context?.User?.FindFirst(x => x.Type == ClaimTypes.Email)?.Value;
            var routeValue = httpContext?.Request?.RouteValues["email"] as string;
            return claim == routeValue;
        }
    }
}