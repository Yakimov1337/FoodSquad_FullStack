using FoodSquad_API.Models.DTO.User;
using FoodSquad_API.Models.Entity;
using FoodSquad_API.Models.Enums;
using FoodSquad_API.Repositories.Interfaces;
using FoodSquad_API.Services.Interfaces;
using System.Security.Claims;

namespace FoodSquad_API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenRepository _tokenRepository;

        public AuthService(
            IUserRepository userRepository,
            ITokenRepository tokenRepository)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
        }

        public async Task<UserResponseDTO> RegisterUserAsync(UserRegistrationDTO userRegistrationDTO)
        {
            // Check if email already exists
            if (await _userRepository.GetByEmailAsync(userRegistrationDTO.Email) != null)
            {
                throw new Exception("Email already exists");
            }

            // Create a new user
            var user = new User
            {
                Email = userRegistrationDTO.Email,
                Password = userRegistrationDTO.Password, // TODO: Hash this password, Store plain text password for now
                Role = UserRole.Normal // Default role
            };

            // Save user
            await _userRepository.AddUserAsync(user);

            // Return the response DTO
            return new UserResponseDTO
            {
                Id = user.Id.ToString(),
                Email = user.Email,
                Name = user.Name,
                Role = user.Role.ToString()
            };
        }

        public async Task<UserResponseDTO> LoginUserAsync(UserLoginDTO userLoginDTO)
        {
            // Fetch user by email
            var user = await _userRepository.GetByEmailAsync(userLoginDTO.Email);
            if (user == null || user.Password != userLoginDTO.Password) // TODO: Use hashed passwords
            {
                throw new Exception("Invalid email or password");
            }

            // Ensure all required user fields are populated
            if (string.IsNullOrEmpty(user.Email) ||
                string.IsNullOrEmpty(user.Role.ToString()) ||
                string.IsNullOrEmpty(user.Name) ||
                string.IsNullOrEmpty(user.PhoneNumber) ||
                string.IsNullOrEmpty(user.ImageUrl))
            {
                throw new Exception("User data contains null fields. Cannot generate token.");
            }

            return new UserResponseDTO
            {
                Id = user.Id.ToString(),
                Email = user.Email,
                Name = user.Name,
                Role = user.Role.ToString(), // Convert to uppercase
                PhoneNumber = user.PhoneNumber,
                ImageUrl = user.ImageUrl
            };
        }


        public async Task<UserResponseDTO> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
        {
            var email = userPrincipal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                throw new UnauthorizedAccessException("Invalid or missing email claim.");
            }

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            return new UserResponseDTO
            {
                Id = user.Id.ToString(),
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                ImageUrl = user.ImageUrl,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task<User> LoadUserEntityByUsernameAsync(string email)
        {
            // Load user by email
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                throw new Exception($"User not found with email: {email}");
            }
            return user;
        }

    }
}
