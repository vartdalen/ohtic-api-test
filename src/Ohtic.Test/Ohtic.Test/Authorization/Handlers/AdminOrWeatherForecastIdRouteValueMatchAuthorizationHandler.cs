using Microsoft.AspNetCore.Authorization;
using Ohtic.Test.Products.Authorization.Requirements;
using Ohtic.Test.Services.Abstractions;

namespace Ohtic.Test.Products.Authorization.Handlers
{
	public class AdminOrWeatherForecastIdRouteValueMatchAuthorizationHandler :
        AuthorizationHandler<AdminOrWeatherForecastIdRouteValueMatchRequirement>
    {
		private readonly ICustomerService _customerService;
		public AdminOrWeatherForecastIdRouteValueMatchAuthorizationHandler(ICustomerService customerService)
		{
			_customerService = customerService;
		}

		protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AdminOrWeatherForecastIdRouteValueMatchRequirement requirement
        )
        {
            if (context.User.Identity?.IsAuthenticated is true &&
                (context.User.IsInRole("admin") is true || await IsResourceOwner(context)))
                context.Succeed(requirement);
            else context.Fail();
            await Task.CompletedTask;
        }

        private async Task<bool> IsResourceOwner(AuthorizationHandlerContext context)
        {
            var httpContext = context.Resource as HttpContext;
            var customerIdClaim = context?.User?.FindFirst(x => x.Type == "customer_id")?.Value;
			var resourceIdRouteValue = httpContext?.Request.RouteValues["id"] as string;
			try
			{
				return await _customerService.IsResourceOwner(
					int.Parse(customerIdClaim!),
					int.Parse(resourceIdRouteValue!)
				);
			}
			catch (KeyNotFoundException) { return false; }
        }
    }
}