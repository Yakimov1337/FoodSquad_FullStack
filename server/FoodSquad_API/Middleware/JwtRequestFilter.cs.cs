using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FoodSquad_API.Services;

public class JwtRequestFilter : IMiddleware
{
    private readonly JwtUtil _jwtUtil;
    private readonly AuthService _authService;
    private readonly TokenService _tokenService;

    public JwtRequestFilter(JwtUtil jwtUtil, AuthService authService, TokenService tokenService)
    {
        _jwtUtil = jwtUtil;
        _authService = authService;
        _tokenService = tokenService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        var requestPath = context.Request.Path.ToString();

        // Skip JWT validation for specific endpoints (e.g., /api/auth/**)
        if (requestPath.StartsWith("/api/auth"))
        {
            await next(context);
            return;
        }

        string token = null;
        if (authorizationHeader != null && authorizationHeader.StartsWith("Bearer "))
        {
            token = authorizationHeader.Substring(7);
        }

        if (token != null)
        {
            try
            {
                var username = _jwtUtil.ExtractUsername(token);

                // Ensure the token is valid and not expired
                if (_jwtUtil.ValidateToken(token) && _tokenService.IsValidToken(token))
                {
                    var user = _authService.LoadUserByUsername(username);

                    var claims = new ClaimsPrincipal(new ClaimsIdentity(user.Claims, "Bearer"));
                    context.User = claims;
                }
            }
            catch (SecurityTokenExpiredException)
            {
                context.Items["expired"] = "Token expired";
            }
            catch (SecurityTokenException)
            {
                context.Items["invalid"] = "Invalid token";
            }
        }

        await next(context);
    }
}
