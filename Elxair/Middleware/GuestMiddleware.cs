using Elxair.Models;

namespace Elxair.Middleware
{
    public class GuestMiddleware
    {
        private readonly RequestDelegate _next;

        public GuestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            if (path.StartsWith("/account/login") ||
                path.StartsWith("/account/register") ||
                path.StartsWith("/account/logout"))
            {
                await _next(context);
                return;
            }
            int? userId = context.Session.GetInt32("UserId");

            if (userId == null)
            {
                context.Session.SetString("IsGuest", "true");

                if (string.IsNullOrEmpty(context.Session.GetString("GuestToken")))
                {
                    context.Session.SetString("GuestToken", Guid.NewGuid().ToString());
                }
            }

            await _next(context);
        }
    }
}