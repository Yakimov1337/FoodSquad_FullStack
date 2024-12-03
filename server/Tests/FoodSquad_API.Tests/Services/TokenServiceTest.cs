using FoodSquad_API.Models.Entity;
using FoodSquad_API.Repositories.Interfaces;
using FoodSquad_API.Services;
using Moq;
using Xunit;

namespace FoodSquad_API.Tests.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<ITokenRepository> _tokenRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _tokenRepositoryMock = new Mock<ITokenRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();

            _tokenService = new TokenService(
                _tokenRepositoryMock.Object,
                _userRepositoryMock.Object
            );
        }

        [Fact]
        public async Task IsRefreshTokenValidAsync_ShouldReturnTrue_WhenTokenIsValid()
        {
            // Arrange
            var email = "test@example.com";
            var refreshToken = "valid-refresh-token";
            var userId = Guid.NewGuid();

            var user = new User { Id = userId, Email = email };
            var token = new Token
            {
                RefreshToken = refreshToken,
                RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(1)
            };

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(user);
            _tokenRepositoryMock.Setup(x => x.FindByUserAndRefreshTokenAsync(userId, refreshToken))
                .ReturnsAsync(token);

            // Act
            var result = await _tokenService.IsRefreshTokenValidAsync(email, refreshToken);

            // Assert
            Assert.True(result);
            _userRepositoryMock.Verify(x => x.GetByEmailAsync(email), Times.Once);
            _tokenRepositoryMock.Verify(x => x.FindByUserAndRefreshTokenAsync(userId, refreshToken), Times.Once);
        }

        [Fact]
        public async Task IsRefreshTokenValidAsync_ShouldReturnFalse_WhenTokenIsExpired()
        {
            // Arrange
            var email = "test@example.com";
            var refreshToken = "expired-refresh-token";
            var userId = Guid.NewGuid();

            var user = new User { Id = userId, Email = email };
            var token = new Token
            {
                RefreshToken = refreshToken,
                RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(-1) // Expired token
            };

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(user);
            _tokenRepositoryMock.Setup(x => x.FindByUserAndRefreshTokenAsync(userId, refreshToken))
                .ReturnsAsync(token);

            // Act
            var result = await _tokenService.IsRefreshTokenValidAsync(email, refreshToken);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SaveTokensAsync_ShouldSaveTokens_WhenUserExists()
        {
            // Arrange
            var email = "test@example.com";
            var accessToken = "new-access-token";
            var refreshToken = "new-refresh-token";
            var userId = Guid.NewGuid();

            var user = new User { Id = userId, Email = email };

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(user);
            _tokenRepositoryMock.Setup(x => x.DeleteByUserAsync(userId)).Returns(Task.CompletedTask);
            _tokenRepositoryMock.Setup(x => x.SaveAsync(It.IsAny<Token>())).Returns(Task.CompletedTask);

            // Act
            await _tokenService.SaveTokensAsync(email, accessToken, refreshToken);

            // Assert
            _userRepositoryMock.Verify(x => x.GetByEmailAsync(email), Times.Once);
            _tokenRepositoryMock.Verify(x => x.DeleteByUserAsync(userId), Times.Once);
            _tokenRepositoryMock.Verify(x => x.SaveAsync(It.IsAny<Token>()), Times.Once);
        }

        [Fact]
        public async Task InvalidateTokensAsync_ShouldDeleteTokens_WhenTokensAreProvided()
        {
            // Arrange
            var accessToken = "valid-access-token";
            var refreshToken = "valid-refresh-token";

            _tokenRepositoryMock.Setup(x => x.DeleteByAccessTokenAsync(accessToken)).Returns(Task.CompletedTask);
            _tokenRepositoryMock.Setup(x => x.DeleteByRefreshTokenAsync(refreshToken)).Returns(Task.CompletedTask);

            // Act
            await _tokenService.InvalidateTokensAsync(accessToken, refreshToken);

            // Assert
            _tokenRepositoryMock.Verify(x => x.DeleteByAccessTokenAsync(accessToken), Times.Once);
            _tokenRepositoryMock.Verify(x => x.DeleteByRefreshTokenAsync(refreshToken), Times.Once);
        }

        [Fact]
        public async Task IsAccessTokenValidAsync_ShouldReturnTrue_WhenTokenIsValid()
        {
            // Arrange
            var accessToken = "valid-access-token";

            var token = new Token
            {
                AccessToken = accessToken,
                AccessTokenExpiryDate = DateTime.UtcNow.AddMinutes(30) // Not expired
            };

            _tokenRepositoryMock.Setup(x => x.FindByAccessTokenAsync(accessToken)).ReturnsAsync(token);

            // Act
            var result = await _tokenService.IsAccessTokenValidAsync(accessToken);

            // Assert
            Assert.True(result);
            _tokenRepositoryMock.Verify(x => x.FindByAccessTokenAsync(accessToken), Times.Once);
        }

        [Fact]
        public async Task IsAccessTokenValidAsync_ShouldReturnFalse_WhenTokenIsExpired()
        {
            // Arrange
            var accessToken = "expired-access-token";

            var token = new Token
            {
                AccessToken = accessToken,
                AccessTokenExpiryDate = DateTime.UtcNow.AddMinutes(-30) // Expired
            };

            _tokenRepositoryMock.Setup(x => x.FindByAccessTokenAsync(accessToken)).ReturnsAsync(token);

            // Act
            var result = await _tokenService.IsAccessTokenValidAsync(accessToken);

            // Assert
            Assert.False(result);
        }
    }
}
