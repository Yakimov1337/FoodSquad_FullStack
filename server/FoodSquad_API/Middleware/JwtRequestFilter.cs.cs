using FoodSquad_API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

public class JwtRequestFilter : IMiddleware
{
    private readonly JwtUtil _jwtUtil;

    public JwtRequestFilter(JwtUtil jwtUtil)
    {
        _jwtUtil = jwtUtil;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var claimsDict = _jwtUtil.ExtractClaims(token);

                var claims = claimsDict.Select(c => new Claim(c.Key, c.Value.ToString())).ToList();

                // Map "role" to ClaimTypes.Role explicitly
                var roleClaim = claims.FirstOrDefault(c => c.Type == "role");
                if (roleClaim != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleClaim.Value)); // Map to "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                }
                var emailClaim = claimsDict.FirstOrDefault(c => c.Key == "email");
                if (emailClaim.Key != null)
                {
                    claims.Add(new Claim(ClaimTypes.Email, emailClaim.Value.ToString()));
                }

                var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
                context.User = new ClaimsPrincipal(identity);

                //Console.WriteLine("[DEBUG] Claims added to HttpContext.User:");
                //foreach (var claim in claims)
                //{
                //    Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
                //}
            }
            catch
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid token");
                return;
            }
        }

        await next(context);
    }
}
