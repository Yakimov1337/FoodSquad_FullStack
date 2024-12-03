using AutoMapper;
using FoodSquad_API.Models.DTO.User;
using FoodSquad_API.Models.Entity;
using FoodSquad_API.Models.Enums;
using FoodSquad_API.Repositories.Interfaces;
using FoodSquad_API.Services;
using FoodSquad_API.Services.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FoodSquad_API.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IReviewRepository> _reviewRepositoryMock;
        private readonly Mock<IMenuItemRepository> _menuItemRepositoryMock;
        private readonly Mock<IUserContextService> _userContextServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _menuItemRepositoryMock = new Mock<IMenuItemRepository>();
            _userContextServiceMock = new Mock<IUserContextService>();
            _mapperMock = new Mock<IMapper>();

            _userService = new UserService(
                _userRepositoryMock.Object,
                _orderRepositoryMock.Object,
                _reviewRepositoryMock.Object,
                _menuItemRepositoryMock.Object,
                _userContextServiceMock.Object,
                _mapperMock.Object,
                null // Optional DbContext if needed
            );
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnUserDTOs_WhenUsersExist()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Email = "user1@test.com" },
                new User { Id = Guid.NewGuid(), Email = "user2@test.com" }
            };

            _userRepositoryMock.Setup(x => x.GetAllPaginatedAsync(1, 10)).ReturnsAsync(users);
            _mapperMock.Setup(x => x.Map<UserResponseDTO>(It.IsAny<User>()))
                       .Returns((User user) => new UserResponseDTO
                       {
                           Id = user.Id.ToString(),
                           Email = user.Email
                       });

            // Act
            var result = await _userService.GetAllUsersAsync(1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(users.Count, result.Count);
            _userRepositoryMock.Verify(x => x.GetAllPaginatedAsync(1, 10), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUserDTO_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Email = "user@test.com", Role = UserRole.Normal };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
            _userContextServiceMock.Setup(x => x.CheckOwnershipAsync(user)).Returns(Task.CompletedTask);
            _mapperMock.Setup(x => x.Map<UserResponseDTO>(user)).Returns(new UserResponseDTO
            {
                Id = user.Id.ToString(),
                Email = user.Email
            });

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id.ToString(), result.Id);
            _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
        }

    }
}
