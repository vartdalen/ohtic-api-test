namespace Ohtic.Test.Products.Middleware
{
    internal sealed class AuthenticatedMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        public AuthenticatedMiddleware(
            RequestDelegate next,
            IConfiguration config
        )
        {
            _next = next;
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                context.Response.Headers.Add(
                    "X-Logout-Url",
                    _config["AppSettings:OAuth:LogoutUri"]
                );
            }

            await _next(context);
        }
    }
}