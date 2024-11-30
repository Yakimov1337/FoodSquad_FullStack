using FoodSquad_API.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

public class JwtRequestFilter : IMiddleware
{
    private readonly JwtUtil _jwtUtil;
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;

    public JwtRequestFilter(JwtUtil jwtUtil, IAuthService authService, ITokenService tokenService)
    {
        _jwtUtil = jwtUtil;
        _authService = authService;
        _tokenService = tokenService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        var requestPath = context.Request.Path.ToString();

        // Skip validation for auth-related endpoints
        if (requestPath.StartsWith("/api/auth") || requestPath.StartsWith("/api/tokens"))
        {
            await next(context);
            return;
        }

        if (authorizationHeader == null || !authorizationHeader.StartsWith("Bearer "))
        {
            Console.WriteLine($"[DEBUG] Invalid or missing Authorization header for path: {requestPath}");
            await next(context);
            return;
        }

        var token = authorizationHeader.Substring(7);

        try
        {
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("[DEBUG] Token is null or empty.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid token.");
                return;
            }

            var claims = _jwtUtil.ExtractClaims(token);
            if (claims == null)
            {
                Console.WriteLine("[DEBUG] Failed to extract claims from token.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid token: Failed to extract claims.");
                return;
            }

            var email = claims.TryGetValue("email", out var emailValue) ? emailValue?.ToString() : null;
            var role = claims.TryGetValue("role", out var roleValue) ? roleValue?.ToString() : null;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
            {
                Console.WriteLine("[DEBUG] Missing required claims. Email or Role is null.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid token: Missing required claims.");
                return;
            }

            // Attach claims to the user context
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            }, "Bearer");

            context.User = new ClaimsPrincipal(identity);

            Console.WriteLine($"[DEBUG] Token validated. User Email: {email}, Role: {role}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DEBUG] Exception during token validation: {ex.Message}");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("An internal error occurred while processing the request.");
            return;
        }

        await next(context);
    }
}
