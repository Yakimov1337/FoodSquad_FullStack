using FoodSquad_API.Models.DTO;

namespace FoodSquad_API.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDTO> CreateOrderAsync(OrderDTO orderDTO);
        Task<List<OrderDTO>> GetAllOrdersAsync(int page, int size);
        Task<List<OrderDTO>> GetOrdersByUserIdAsync(Guid userId, int page, int size);
        Task<OrderDTO> GetOrderByIdAsync(Guid id);
        Task<OrderDTO> UpdateOrderAsync(Guid id, OrderDTO orderDTO);
        Task DeleteOrderAsync(Guid id);
        Task DeleteOrdersAsync(List<Guid> ids);
    }
}
