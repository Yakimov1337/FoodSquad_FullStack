using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FoodSquad_API.Services.Interfaces;
using FoodSquad_API.Models.Enums;
using Swashbuckle.AspNetCore.Annotations;
using FoodSquad_API.Models.DTO.User;
using FoodSquad_API.Services;

namespace FoodSquad_API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly JwtUtil _jwtUtil;

        public AuthController(IAuthService authService, ITokenService tokenService, JwtUtil jwtUtil)
        {
            _authService = authService;
            _tokenService = tokenService;
            _jwtUtil = jwtUtil;
        }

        [HttpPost("sign-up")]
        [SwaggerOperation(
            Summary = "User registration",
            Description = "Registers a new user with the provided details, including validation for password confirmation."
        )]
        [SwaggerResponse(200, "User successfully registered.", typeof(UserResponseDTO))]
        [SwaggerResponse(400, "Validation error.")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDTO userRegistrationDTO)
        {
            if (userRegistrationDTO.Password != userRegistrationDTO.ConfirmPassword)
            {
                return BadRequest("Passwords do not match");
            }

            var user = await _authService.RegisterUserAsync(userRegistrationDTO);
            return Ok(user);
        }

        [HttpPost("sign-in")]
        [SwaggerOperation(
            Summary = "User Login",
            Description = "Login to via email to obtain access token."
        )]
        public async Task<IActionResult> LoginUser([FromBody] UserLoginDTO userLoginDTO)
        {
            // Authenticate user
            var user = await _authService.LoginUserAsync(userLoginDTO);

            // Generate tokens
            var accessToken = _jwtUtil.GenerateToken(
                user.Email,
                Enum.Parse<UserRole>(user.Role, true),
                Guid.Parse(user.Id),
                user.Name,
                user.PhoneNumber,
                user.ImageUrl,
                _jwtUtil.AccessTokenExpiration);

            var refreshToken = _jwtUtil.GenerateToken(
                user.Email,
                Enum.Parse<UserRole>(user.Role, true),
                Guid.Parse(user.Id),
                user.Name,
                user.PhoneNumber,
                user.ImageUrl,
                _jwtUtil.RefreshTokenExpiration);

            // Save tokens
            await _tokenService.SaveTokensAsync(user.Email, accessToken, refreshToken);

            // Set refresh token in cookie
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                MaxAge = TimeSpan.FromMilliseconds(_jwtUtil.RefreshTokenExpiration)
            });

            return Ok(new { accessToken });
        }








        [HttpPost("logout")]
        [SwaggerOperation(
            Summary = "Logout user",
            Description = "Invalidates the user's access and refresh tokens, logging them out of the application."
        )]
        [SwaggerResponse(200, "User successfully logged out.")]
        [SwaggerResponse(500, "Server error during logout.")]
        public async Task<IActionResult> LogoutUser(
    [FromHeader(Name = "Authorization")] string authorizationHeader)
        {
            // Extract Access Token from Authorization header
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return BadRequest(new { error = "Authorization header is missing or invalid." });
            }

            var accessToken = authorizationHeader.Substring("Bearer ".Length);

            // Extract Refresh Token from Cookies (Optional)
            var refreshToken = Request.Cookies.ContainsKey("refreshToken") ? Request.Cookies["refreshToken"] : null;

            // Invalidate the tokens even if refreshToken is missing
            await _tokenService.InvalidateTokensAsync(accessToken, refreshToken);

            // Clear Refresh Token Cookie
            Response.Cookies.Append("refreshToken", string.Empty, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow
            });

            return Ok(new { message = "Successfully logged out." });
        }



        [HttpGet("current-user")]
        [SwaggerOperation(
          Summary = "Get current user",
          Description = "Fetches details of the currently authenticated user based on the provided token."
      )]
        [SwaggerResponse(200, "Authenticated user's details.", typeof(UserResponseDTO))]
        [SwaggerResponse(401, "Unauthorized or invalid token.")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            if (string.IsNullOrEmpty(email))
            {
                Console.WriteLine("Email claim not found.");
                return Unauthorized(new { error = "Invalid or missing email claim." });
            }

            var user = await _authService.LoadUserEntityByUsernameAsync(email);
            return Ok(new UserResponseDTO(user));
        }
    }
}
