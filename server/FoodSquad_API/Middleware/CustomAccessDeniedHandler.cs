using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;
using System.Net;
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
            // Set response details
            context.Response.ContentType = "application/json";

            if (context.User.Identity?.IsAuthenticated != true)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    error = "Unauthorized",
                    message = "Access token is missing or expired."
                }));
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                error = "Access denied.",
                message = "You do not have permission to access this resource."
            }));
            return;
        }

        await _defaultHandler.HandleAsync(next, context, policy, authorizationResult);
    }
}
