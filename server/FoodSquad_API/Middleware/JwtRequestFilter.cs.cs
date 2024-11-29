using FoodSquad_API.Services.Interfaces;
using FoodSquad_API.Utils;
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

        // Skip token validation for specific endpoints like auth
        if (requestPath.StartsWith("/api/auth") || requestPath.StartsWith("/api/token"))
        {
            await next(context);
            return;
        }

        string token = null;
        if (authorizationHeader != null && authorizationHeader.StartsWith("Bearer "))
        {
            token = authorizationHeader.Substring(7);
        }

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var claims = _jwtUtil.ExtractClaims(token);
                var email = claims["sub"].ToString();

                // Load user details and validate token
                var userDetails = await _authService.LoadUserEntityByUsernameAsync(email);
                if (_jwtUtil.ValidateToken(token, userDetails) &&
                    await _tokenService.IsRefreshTokenValidAsync(email, token))
                {
                    var identity = new ClaimsIdentity(claims.Select(c => new Claim(c.Key, c.Value.ToString())), "Bearer");
                    context.User = new ClaimsPrincipal(identity);
                }
            }
            catch (SecurityTokenException)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid token");
                return;
            }
        }

        await next(context);
    }
}
