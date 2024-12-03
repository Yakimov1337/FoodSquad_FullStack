using AutoMapper;
using FoodSquad_API.Models.DTO.Order;
using FoodSquad_API.Models.Entity;
using FoodSquad_API.Repositories.Interfaces;
using FoodSquad_API.Services;
using FoodSquad_API.Services.Interfaces;
using Moq;
using Xunit;

namespace FoodSquad_API.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IMenuItemRepository> _menuItemRepositoryMock;
        private readonly Mock<IUserContextService> _userContextServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _menuItemRepositoryMock = new Mock<IMenuItemRepository>();
            _userContextServiceMock = new Mock<IUserContextService>();
            _mapperMock = new Mock<IMapper>();

            _orderService = new OrderService(
                _orderRepositoryMock.Object,
                _menuItemRepositoryMock.Object,
                _userContextServiceMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldCreateOrder_WhenInputIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var currentUser = new User { Id = userId, Name = "Test User" };
            var menuItemId = 1;

            var orderCreateDTO = new OrderCreateDTO
            {
                MenuItemQuantities = new Dictionary<long, int> { { menuItemId, 2 } }
            };

            var menuItem = new MenuItem { Id = menuItemId, Price = 10.0 };
            var order = new Order
            {
                Id = Guid.NewGuid(),
                TotalCost = 20.0,
                User = currentUser
            };

            var orderDTO = new OrderDTO
            {
                Id = order.Id,
                TotalCost = order.TotalCost
            };

            _userContextServiceMock.Setup(x => x.GetCurrentUserAsync()).ReturnsAsync(currentUser);
            _menuItemRepositoryMock.Setup(x => x.GetByIdAsync(menuItemId)).ReturnsAsync(menuItem);
            _mapperMock.Setup(x => x.Map<Order>(orderCreateDTO)).Returns(order);
            _orderRepositoryMock.Setup(x => x.CreateOrderAsync(order)).Returns(Task.CompletedTask);
            _mapperMock.Setup(x => x.Map<OrderDTO>(order)).Returns(orderDTO);

            // Act
            var result = await _orderService.CreateOrderAsync(orderCreateDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderDTO.Id, result.Id);
            Assert.Equal(20.0, result.TotalCost);

            _orderRepositoryMock.Verify(x => x.CreateOrderAsync(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public async Task GetAllOrdersAsync_ShouldReturnOrders_WhenOrdersExist()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { Id = Guid.NewGuid(), TotalCost = 50.0 },
                new Order { Id = Guid.NewGuid(), TotalCost = 100.0 }
            };

            var orderDTOs = orders.Select(o => new OrderDTO { Id = o.Id, TotalCost = o.TotalCost }).ToList();

            _orderRepositoryMock.Setup(x => x.GetPagedOrdersAsync(1, 10)).ReturnsAsync(orders);
            _mapperMock.Setup(x => x.Map<List<OrderDTO>>(orders)).Returns(orderDTOs);

            // Act
            var result = await _orderService.GetAllOrdersAsync(1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orders.Count, result.Count);

            _orderRepositoryMock.Verify(x => x.GetPagedOrdersAsync(1, 10), Times.Once);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                TotalCost = 50.0,
                User = new User { Id = Guid.NewGuid() }
            };

            var orderDTO = new OrderDTO { Id = order.Id, TotalCost = order.TotalCost };

            _orderRepositoryMock.Setup(x => x.GetOrderWithDetailsAsync(orderId)).ReturnsAsync(order);
            _userContextServiceMock.Setup(x => x.CheckOwnershipAsync(order.User)).Returns(Task.CompletedTask);
            _mapperMock.Setup(x => x.Map<OrderDTO>(order)).Returns(orderDTO);

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderDTO.Id, result.Id);

            _orderRepositoryMock.Verify(x => x.GetOrderWithDetailsAsync(orderId), Times.Once);
        }

        [Fact]
        public async Task DeleteOrderAsync_ShouldDeleteOrder_WhenOrderExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                User = new User { Id = Guid.NewGuid() }
            };

            _orderRepositoryMock.Setup(x => x.GetOrderWithDetailsAsync(orderId)).ReturnsAsync(order);
            _userContextServiceMock.Setup(x => x.CheckOwnershipAsync(order.User)).Returns(Task.CompletedTask);
            _orderRepositoryMock.Setup(x => x.DeleteOrderAsync(orderId)).Returns(Task.CompletedTask);

            // Act
            var result = await _orderService.DeleteOrderAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(orderId.ToString(), result);

            _orderRepositoryMock.Verify(x => x.DeleteOrderAsync(orderId), Times.Once);
        }
    }
}
