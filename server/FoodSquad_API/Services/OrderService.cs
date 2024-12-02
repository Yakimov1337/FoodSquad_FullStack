using AutoMapper;
using FoodSquad_API.Models.DTO.Order;
using FoodSquad_API.Models.Entity;
using FoodSquad_API.Models.Enums;
using FoodSquad_API.Repositories.Interfaces;
using FoodSquad_API.Services.Interfaces;

namespace FoodSquad_API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IMenuItemRepository menuItemRepository,
            IUserContextService userContextService,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _menuItemRepository = menuItemRepository;
            _userContextService = userContextService;
            _mapper = mapper;
        }

        // Create Order
        public async Task<OrderDTO> CreateOrderAsync(OrderCreateDTO orderCreateDTO)
        {
            if (orderCreateDTO.MenuItemQuantities == null || !orderCreateDTO.MenuItemQuantities.Any())
                throw new ArgumentException("Order must contain at least one menu item.");

            var currentUser = await _userContextService.GetCurrentUserAsync();

            double totalCost = 0;
            var menuItemsWithQuantity = new List<OrderMenuItem>();

            foreach (var entry in orderCreateDTO.MenuItemQuantities)
            {
                var menuItem = await _menuItemRepository.GetByIdAsync(entry.Key);
                if (menuItem == null)
                    throw new ArgumentException($"Invalid menu item ID: {entry.Key}");

                menuItemsWithQuantity.Add(new OrderMenuItem
                {
                    MenuItem = menuItem,
                    Quantity = entry.Value
                });

                totalCost += menuItem.Price * entry.Value;
            }

            var order = _mapper.Map<Order>(orderCreateDTO);
            order.User = currentUser;
            order.MenuItemsWithQuantity = menuItemsWithQuantity;
            order.TotalCost = Math.Round(totalCost, 2);

            await _orderRepository.CreateOrderAsync(order);

            return _mapper.Map<OrderDTO>(order);
        }


        // Get All Orders (Paginated)
        public async Task<List<OrderDTO>> GetAllOrdersAsync(int page, int size)
        {
            var orders = await _orderRepository.GetPagedOrdersAsync(page, size);
            return orders.Select(order => _mapper.Map<OrderDTO>(order)).ToList();
        }

        // Get Orders By User ID (Paginated)
        public async Task<List<OrderDTO>> GetOrdersByUserIdAsync(Guid userId, int page, int size)
        {
            var user = await _userContextService.GetCurrentUserAsync();
            if (user.Id != userId)
                await _userContextService.CheckOwnershipAsync(user);

            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId, page, size);
            return orders.Select(order => _mapper.Map<OrderDTO>(order)).ToList();
        }

        // Get Order By ID
        public async Task<OrderDTO> GetOrderByIdAsync(Guid id)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(id);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {id} not found.");

            await _userContextService.CheckOwnershipAsync(order.User);

            return _mapper.Map<OrderDTO>(order);
        }

        // Update Order
        public async Task<OrderDTO> UpdateOrderAsync(Guid id, OrderUpdateDTO orderUpdateDTO)
        {
            if (orderUpdateDTO.MenuItemQuantities == null || !orderUpdateDTO.MenuItemQuantities.Any())
                throw new ArgumentException("Order must contain at least one menu item.");

            var existingOrder = await _orderRepository.GetOrderWithDetailsAsync(id);
            if (existingOrder == null)
                throw new KeyNotFoundException($"Order with ID {id} not found.");

            await _userContextService.CheckOwnershipAsync(existingOrder.User);

            double totalCost = 0;
            var menuItemsWithQuantity = new List<OrderMenuItem>();

            foreach (var entry in orderUpdateDTO.MenuItemQuantities)
            {
                var menuItem = await _menuItemRepository.GetByIdAsync(entry.Key);
                if (menuItem == null)
                    throw new ArgumentException($"Invalid menu item ID: {entry.Key}");

                menuItemsWithQuantity.Add(new OrderMenuItem
                {
                    MenuItem = menuItem,
                    Quantity = entry.Value
                });

                totalCost += menuItem.Price * entry.Value;
            }

            _mapper.Map(orderUpdateDTO, existingOrder);
            existingOrder.MenuItemsWithQuantity = menuItemsWithQuantity;
            existingOrder.TotalCost = Math.Round(totalCost, 2);

            await _orderRepository.UpdateOrderAsync(existingOrder);

            return _mapper.Map<OrderDTO>(existingOrder);
        }


        public async Task<string> DeleteOrderAsync(Guid id)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(id);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {id} not found.");

            await _userContextService.CheckOwnershipAsync(order.User);

            await _orderRepository.DeleteOrderAsync(id);

            return $"Order with ID {id} was successfully deleted.";
        }

        public async Task<string> DeleteOrdersAsync(List<Guid> ids)
        {
            var orders = await _orderRepository.GetOrdersByIdsAsync(ids);
            if (orders == null || !orders.Any())
                throw new KeyNotFoundException("No orders found for the provided IDs.");

            foreach (var order in orders)
            {
                await _userContextService.CheckOwnershipAsync(order.User);
            }

            await _orderRepository.DeleteOrdersAsync(ids);

            return $"Orders were successfully deleted.";
        }



    }
}
