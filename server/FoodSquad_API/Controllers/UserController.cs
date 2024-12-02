using FoodSquad_API.Models.DTO.User;
using FoodSquad_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace FoodSquad_API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public UserController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new user",
            Description = "Allows only admins to create a new user by providing user registration details."
        )]

        public async Task<IActionResult> CreateUser([FromBody] UserRegistrationDTO userRegistrationDTO)
        {
            var user = await _authService.RegisterUserAsync(userRegistrationDTO);
            return Ok(user);
        }

        [Authorize(Policy = "ModeratorPolicy")]
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all users",
            Description = "Allows admins and moderators to view all users with optional pagination."
        )]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 0, [FromQuery] int size = 10)
        {
            var users = await _userService.GetAllUsersAsync(page, size);
            return Ok(users);
        }

        [Authorize(Policy = "ModeratorPolicy")]
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get user by ID",
            Description = "Allows admins and moderators to view details of a specific user by their ID."
        )]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Update user information",
            Description = "Allows admins, moderators, and users to update their own information. Admins can update any user."
        )]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateDTO userUpdateDTO)
        {
            var updatedUser = await _userService.UpdateUserAsync(id, userUpdateDTO);
            return Ok(updatedUser);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete a user",
            Description = "Allows only admins to delete a user by their ID."
        )]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok(new { message = "User successfully deleted." });
        }
    }
}
