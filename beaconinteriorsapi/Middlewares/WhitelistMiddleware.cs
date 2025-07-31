namespace beaconinteriorsapi.Middlewares
{

    public class WhitelistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HashSet<string> _allowedDomains;

        public WhitelistMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            // Load allowed domains from configuration or hard-code them
            _allowedDomains = config.GetSection("AllowedDomains").Get<HashSet<string>>()
                              ?? new HashSet<string> { "https://example.com", "http://localhost:3000" };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var origin = context.Request.Headers["Origin"].ToString();
            var referer = context.Request.Headers["Referer"].ToString();

            var isAllowed = _allowedDomains.Any(domain =>
                (!string.IsNullOrWhiteSpace(origin) && origin.StartsWith(domain)) ||
                (!string.IsNullOrWhiteSpace(referer) && referer.StartsWith(domain)));

            if (!isAllowed)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden: Your domain is not whitelisted.");
                return;
            }

            await _next(context);
        }
    }

    public static class WhitelistMiddlewareExtensions
    {
        public static IApplicationBuilder UseWhitelist(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WhitelistMiddleware>();
        }
    }

}
