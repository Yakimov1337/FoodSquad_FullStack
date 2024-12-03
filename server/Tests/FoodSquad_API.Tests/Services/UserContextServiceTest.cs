using FoodSquad_API.Models.Entity;
using FoodSquad_API.Models.Enums;
using FoodSquad_API.Repositories.Interfaces;
using FoodSquad_API.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using Xunit;

namespace FoodSquad_API.Tests.Services
{
    public class UserContextServiceTests
    {
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserContextService _userContextService;

        public UserContextServiceTests()
        {
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _userContextService = new UserContextService(_httpContextAccessorMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task GetCurrentUserAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Role = UserRole.Normal
            };

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, email)
            }, "TestAuth"));

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = claimsPrincipal });
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(user);

            // Act
            var result = await _userContextService.GetCurrentUserAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.Role, result.Role);
            _userRepositoryMock.Verify(x => x.GetByEmailAsync(email), Times.Once);
        }

        [Fact]
        public async Task GetCurrentUserAsync_ShouldThrowUnauthorizedAccessException_WhenUserNotLoggedIn()
        {
            // Arrange
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = null });

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userContextService.GetCurrentUserAsync());
        }

        [Fact]
        public async Task GetCurrentUserAsync_ShouldThrowUnauthorizedAccessException_WhenUserNotFoundInDatabase()
        {
            // Arrange
            var email = "test@example.com";
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, email)
            }, "TestAuth"));

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = claimsPrincipal });
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userContextService.GetCurrentUserAsync());
        }

        [Fact]
        public async Task CheckOwnershipAsync_ShouldNotThrowException_WhenUserOwnsResource()
        {
            // Arrange
            var currentUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Role = UserRole.Normal
            };

            var resourceOwner = new User
            {
                Id = currentUser.Id,
                Email = "test@example.com",
                Role = UserRole.Normal
            };

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, currentUser.Email)
                }, "TestAuth"))
            });

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(currentUser.Email)).ReturnsAsync(currentUser);

            // Act
            await _userContextService.CheckOwnershipAsync(resourceOwner);

            // Assert
            _userRepositoryMock.Verify(x => x.GetByEmailAsync(currentUser.Email), Times.Once);
        }

        [Fact]
        public async Task CheckOwnershipAsync_ShouldThrowUnauthorizedAccessException_WhenUserDoesNotOwnResource()
        {
            // Arrange
            var currentUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Role = UserRole.Normal
            };

            var resourceOwner = new User
            {
                Id = Guid.NewGuid(),
                Email = "resource@example.com",
                Role = UserRole.Normal
            };

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, currentUser.Email)
                }, "TestAuth"))
            });

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(currentUser.Email)).ReturnsAsync(currentUser);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userContextService.CheckOwnershipAsync(resourceOwner));
        }

        [Fact]
        public async Task CheckOwnershipAsync_ShouldNotThrowException_WhenUserIsAdmin()
        {
            // Arrange
            var currentUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@example.com",
                Role = UserRole.Admin
            };

            var resourceOwner = new User
            {
                Id = Guid.NewGuid(),
                Email = "resource@example.com",
                Role = UserRole.Normal
            };

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, currentUser.Email)
                }, "TestAuth"))
            });

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(currentUser.Email)).ReturnsAsync(currentUser);

            // Act
            await _userContextService.CheckOwnershipAsync(resourceOwner);

            // Assert
            _userRepositoryMock.Verify(x => x.GetByEmailAsync(currentUser.Email), Times.Once);
        }
    }
}
