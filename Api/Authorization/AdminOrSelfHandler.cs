using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api.Authorization
{
    public class AdminOrSelfHandler : AuthorizationHandler<AdminOrSelfRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AdminOrSelfRequirement requirement)
        {
            // Check if user is authenticated
            if (!context.User.Identity?.IsAuthenticated ?? true)
                return Task.CompletedTask;

            // Check if user is Admin
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Get the user ID from the route (must match your endpoint)
            var httpContext = (context.Resource as HttpContext)
                ?? (context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext)?.HttpContext;


            if (httpContext != null)
            {
                var routeId = httpContext.Request.RouteValues["id"]?.ToString();

                // Get user ID from claim
                var userIdClaim = context.User.FindFirst("UserId")?.Value;

                if (routeId != null && routeId == userIdClaim)
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            return Task.CompletedTask;
        }
    }
}
