using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

public class CustomAccessDeniedHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new AuthorizationMiddlewareResultHandler();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizationResult)
    {
        if (!authorizationResult.Succeeded)
        {
            // Log concise failure details
            Console.WriteLine("[DEBUG] Authorization failed.");

            // Log the roles from the user's claims (if available)
            var userRoles = context.User.Claims
                                .Where(c => c.Type == ClaimTypes.Role)
                                .Select(c => c.Value)
                                .ToList();
            Console.WriteLine($"[DEBUG] User Roles: {string.Join(", ", userRoles)}");

            // Log required roles
            var requiredRoles = policy.Requirements
                                      .OfType<RolesAuthorizationRequirement>()
                                      .SelectMany(r => r.AllowedRoles)
                                      .ToList();
            Console.WriteLine($"[DEBUG] Required Roles: {string.Join(", ", requiredRoles)}");

            // Log role mismatches for debugging
            var mismatchedRoles = requiredRoles.Where(r => !userRoles.Contains(r)).ToList();
            if (mismatchedRoles.Any())
            {
                Console.WriteLine($"[DEBUG] Missing Roles: {string.Join(", ", mismatchedRoles)}");
            }

            // Respond with error
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

            var response = new
            {
                error = "Access denied",
                message = "You do not have permission to access this resource"
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            return;
        }

        // Proceed with the default behavior if authorization succeeds
        await _defaultHandler.HandleAsync(next, context, policy, authorizationResult);
    }
}
