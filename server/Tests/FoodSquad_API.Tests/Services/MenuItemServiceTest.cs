using AutoMapper;
using FoodSquad_API.Models.DTO.MenuItem;
using FoodSquad_API.Models.Entity;
using FoodSquad_API.Repositories.Interfaces;
using FoodSquad_API.Services;
using FoodSquad_API.Services.Interfaces;
using Moq;
using Xunit;

namespace FoodSquad_API.Tests.Services
{
    public class MenuItemServiceTests
    {
        private readonly Mock<IMenuItemRepository> _menuItemRepositoryMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IReviewRepository> _reviewRepositoryMock;
        private readonly Mock<IUserContextService> _userContextServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly MenuItemService _menuItemService;

        public MenuItemServiceTests()
        {
            _menuItemRepositoryMock = new Mock<IMenuItemRepository>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _userContextServiceMock = new Mock<IUserContextService>();
            _mapperMock = new Mock<IMapper>();

            _menuItemService = new MenuItemService(
                _menuItemRepositoryMock.Object,
                _orderRepositoryMock.Object,
                _reviewRepositoryMock.Object,
                _userContextServiceMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task CreateMenuItemAsync_ShouldCreateMenuItem_WhenInputIsValid()
        {
            // Arrange
            var currentUser = new User { Id = Guid.NewGuid(), Name = "Test User" };
            var menuItemCreateDTO = new MenuItemCreateDTO { Title = "Test Item" };
            var menuItem = new MenuItem { Id = 1, Title = "Test Item", User = currentUser };
            var menuItemDTO = new MenuItemDTO { Id = 1, Title = "Test Item" };

            _userContextServiceMock.Setup(x => x.GetCurrentUserAsync()).ReturnsAsync(currentUser);
            _mapperMock.Setup(x => x.Map<MenuItem>(menuItemCreateDTO)).Returns(menuItem);
            _menuItemRepositoryMock.Setup(x => x.AddAsync(menuItem)).Returns(Task.CompletedTask);
            _menuItemRepositoryMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(x => x.Map<MenuItemDTO>(menuItem)).Returns(menuItemDTO);

            // Act
            var result = await _menuItemService.CreateMenuItemAsync(menuItemCreateDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(menuItemDTO.Id, result.Id);
            _menuItemRepositoryMock.Verify(x => x.AddAsync(It.IsAny<MenuItem>()), Times.Once);
            _menuItemRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetMenuItemByIdAsync_ShouldReturnMenuItem_WhenItemExists()
        {
            // Arrange
            var menuItem = new MenuItem { Id = 1, Title = "Test Item", User = new User { Id = Guid.NewGuid() } };
            var menuItemDTO = new MenuItemDTO { Id = 1, Title = "Test Item" };

            _menuItemRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(menuItem);
            _userContextServiceMock.Setup(x => x.CheckOwnershipAsync(menuItem.User)).Returns(Task.CompletedTask);
            _mapperMock.Setup(x => x.Map<MenuItemDTO>(menuItem)).Returns(menuItemDTO);
            _orderRepositoryMock.Setup(x => x.SumQuantityByMenuItemIdAsync(1)).ReturnsAsync(10);
            _reviewRepositoryMock.Setup(x => x.CountByMenuItemIdAsync(1)).ReturnsAsync(2);
            _reviewRepositoryMock.Setup(x => x.FindAverageRatingByMenuItemIdAsync(1)).ReturnsAsync(4.5);

            // Act
            var result = await _menuItemService.GetMenuItemByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(menuItemDTO.Id, result.Id);
            _menuItemRepositoryMock.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteMenuItemAsync_ShouldDeleteMenuItem_WhenItemExists()
        {
            // Arrange
            var menuItem = new MenuItem { Id = 1, User = new User { Id = Guid.NewGuid() } };

            _menuItemRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(menuItem);
            _userContextServiceMock.Setup(x => x.CheckOwnershipAsync(menuItem.User)).Returns(Task.CompletedTask);
            _orderRepositoryMock.Setup(x => x.RemoveMenuItemReferencesAsync(1)).Returns(Task.CompletedTask);
            _menuItemRepositoryMock.Setup(x => x.DeleteAsync(menuItem)).Returns(Task.CompletedTask);

            // Act
            var result = await _menuItemService.DeleteMenuItemAsync(1);

            // Assert
            Assert.True(result);
            _menuItemRepositoryMock.Verify(x => x.GetByIdAsync(1), Times.Once);
            _menuItemRepositoryMock.Verify(x => x.DeleteAsync(menuItem), Times.Once);
        }

        [Fact]
        public async Task GetMenuItemsByIdsAsync_ShouldReturnMenuItems_WhenItemsExist()
        {
            // Arrange
            var menuItems = new List<MenuItem>
            {
                new MenuItem { Id = 1, Title = "Test Item 1" },
                new MenuItem { Id = 2, Title = "Test Item 2" }
            };

            var menuItemDTOs = new List<MenuItemDTO>
            {
                new MenuItemDTO { Id = 1, Title = "Test Item 1" },
                new MenuItemDTO { Id = 2, Title = "Test Item 2" }
            };

            _menuItemRepositoryMock.Setup(x => x.GetByIdsAsync(new List<long> { 1, 2 })).ReturnsAsync(menuItems);
            _mapperMock.Setup(x => x.Map<List<MenuItemDTO>>(menuItems)).Returns(menuItemDTOs);

            // Act
            var result = await _menuItemService.GetMenuItemsByIdsAsync(new List<long> { 1, 2 });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(menuItems.Count, result.Count);
            _menuItemRepositoryMock.Verify(x => x.GetByIdsAsync(It.IsAny<List<long>>()), Times.Once);
        }
    }
}
