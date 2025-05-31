using Oceanarium.Servises.Interfaces;

namespace Oceanarium.Middleware
{
    public class AdminAreaMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAdminKeyService _adminKeyService;
        public AdminAreaMiddleware(RequestDelegate next, IAdminKeyService adminKeyService)
        {
            _next = next;
            _adminKeyService = adminKeyService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/Admin", StringComparison.OrdinalIgnoreCase))
            {
                var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false; //null => false
                var isAdmin = context.User.IsInRole("Admin");

                var hasValidAccessCode = context.Request.Cookies.TryGetValue("AdminAccessCode", out var code)
                                         && _adminKeyService.IsValidKey(code);

                if (!(isAuthenticated && isAdmin) && !hasValidAccessCode)
                {
                    context.Response.Redirect("/AccessDenied");
                    return;
                }
            }

            await _next(context);
        }
    }
}