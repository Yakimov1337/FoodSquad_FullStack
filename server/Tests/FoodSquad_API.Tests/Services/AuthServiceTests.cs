using FoodSquad_API.Models.DTO.User;
using FoodSquad_API.Models.Entity;
using FoodSquad_API.Models.Enums;
using FoodSquad_API.Repositories.Interfaces;
using FoodSquad_API.Services;
using Moq;
using System.Security.Claims;
using Xunit;

namespace FoodSquad_API.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ITokenRepository> _mockTokenRepository;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockTokenRepository = new Mock<ITokenRepository>();
            _authService = new AuthService(_mockUserRepository.Object, _mockTokenRepository.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_EmailAlreadyExists_ThrowsException()
        {
            // Arrange
            var existingUser = new User { Email = "test@example.com" };
            _mockUserRepository
                .Setup(repo => repo.GetByEmailAsync(existingUser.Email))
                .ReturnsAsync(existingUser);

            var newUser = new UserRegistrationDTO { Email = "test@example.com", Password = "password123" };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authService.RegisterUserAsync(newUser));
        }

        [Fact]
        public async Task RegisterUserAsync_ValidInput_ReturnsUserResponseDTO()
        {
            // Arrange
            var newUser = new UserRegistrationDTO { Email = "new@example.com", Password = "password123" };
            _mockUserRepository
                .Setup(repo => repo.GetByEmailAsync(newUser.Email))
                .ReturnsAsync((User)null);

            _mockUserRepository
                .Setup(repo => repo.AddUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RegisterUserAsync(newUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newUser.Email, result.Email);
            _mockUserRepository.Verify(repo => repo.AddUserAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task LoginUserAsync_InvalidEmailOrPassword_ThrowsException()
        {
            // Arrange
            var userLogin = new UserLoginDTO { Email = "test@example.com", Password = "wrongpassword" };
            _mockUserRepository
                .Setup(repo => repo.GetByEmailAsync(userLogin.Email))
                .ReturnsAsync(new User { Email = "test@example.com", Password = "correctpassword" });

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authService.LoginUserAsync(userLogin));
        }

        [Fact]
        public async Task LoginUserAsync_ValidInput_ReturnsUserResponseDTO()
        {
            // Arrange
            var userLogin = new UserLoginDTO { Email = "test@example.com", Password = "password123" };
            var user = new User
            {
                Email = "test@example.com",
                Password = "password123",
                Role = UserRole.Normal,
                Name = "Test User",
                PhoneNumber = "1234567890",
                ImageUrl = "image.jpg"
            };

            _mockUserRepository
                .Setup(repo => repo.GetByEmailAsync(userLogin.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginUserAsync(userLogin);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.Name, result.Name);
        }

        [Fact]
        public async Task GetCurrentUserAsync_InvalidClaim_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                // No email claim
            }));

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _authService.GetCurrentUserAsync(claimsPrincipal));
        }

        [Fact]
        public async Task GetCurrentUserAsync_ValidClaim_ReturnsUserResponseDTO()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, "test@example.com")
            }));

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Name = "Test User",
                Role = UserRole.Admin,
                ImageUrl = "image.jpg",
                PhoneNumber = "1234567890"
            };

            _mockUserRepository
                .Setup(repo => repo.GetByEmailAsync("test@example.com"))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.GetCurrentUserAsync(claimsPrincipal);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.Name, result.Name);
        }

        [Fact]
        public async Task LoadUserEntityByUsernameAsync_NonExistingEmail_ThrowsException()
        {
            // Arrange
            var email = "nonexistent@example.com";
            _mockUserRepository
                .Setup(repo => repo.GetByEmailAsync(email))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _authService.LoadUserEntityByUsernameAsync(email));
        }

        [Fact]
        public async Task LoadUserEntityByUsernameAsync_ValidEmail_ReturnsUser()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User { Email = email };
            _mockUserRepository
                .Setup(repo => repo.GetByEmailAsync(email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoadUserEntityByUsernameAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
        }
    }
}
