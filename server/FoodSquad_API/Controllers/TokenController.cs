
using FoodSquad_API;
using FoodSquad_API.Models.Enums;
using FoodSquad_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;

[Route("api/token")]
[ApiController]
public class TokenController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly JwtUtil _jwtUtil;
    private readonly ILogger<TokenController> _logger;

    public TokenController(
        IAuthService authService,
        ITokenService tokenService,
      JwtUtil jwtUtil,
    ILogger<TokenController> logger)
    {
        _authService = authService;
        _tokenService = tokenService;
        _jwtUtil = jwtUtil;
        _logger = logger;
    }

    [HttpPost("refresh-token")]
    [SwaggerOperation(
    Summary = "Refresh Access Token",
    Description = "Generates a new access token and refresh token using a valid refresh token from cookies. Requires the 'refreshToken' cookie to be present and valid."
)]
    public async Task<IActionResult> RefreshToken()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            return Unauthorized(new { error = "Unauthorized", message = "Refresh token is missing. Please log in again." });
        }

        try
        {
            var claims = _jwtUtil.ExtractClaims(refreshToken);
            var email = claims["email"]?.ToString();

            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized(new { error = "Unauthorized", message = "Invalid refresh token: Missing email claim." });
            }

            var user = await _authService.LoadUserEntityByUsernameAsync(email);
            if (user == null || !await _tokenService.IsRefreshTokenValidAsync(email, refreshToken))
            {
                return Unauthorized(new { error = "Unauthorized", message = "Refresh token is invalid or expired." });
            }

            // Generate new tokens
            var newAccessToken = _jwtUtil.GenerateToken(
                email: email,
                role: user.Role,
                id: user.Id,
                name: user.Name,
                phoneNumber: user.PhoneNumber,
                imageUrl: user.ImageUrl,
                expiration: _jwtUtil.AccessTokenExpiration);

            var newRefreshToken = _jwtUtil.GenerateToken(
                email: email,
                role: user.Role,
                id: user.Id,
                name: user.Name,
                phoneNumber: user.PhoneNumber,
                imageUrl: user.ImageUrl,
                expiration: _jwtUtil.RefreshTokenExpiration);

            // Save new tokens
            await _tokenService.SaveTokensAsync(email, newAccessToken, newRefreshToken);

            Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                MaxAge = TimeSpan.FromMilliseconds(_jwtUtil.RefreshTokenExpiration)
            });

            return Ok(new { accessToken = newAccessToken });
        }
        catch (SecurityTokenExpiredException)
        {
            return Unauthorized(new { error = "Unauthorized", message = "Refresh token has expired. Please log in again." });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error refreshing token: {Message}", ex.Message);
            return BadRequest(new { error = "BadRequest", message = "An error occurred while refreshing the token." });
        }
    }




}
