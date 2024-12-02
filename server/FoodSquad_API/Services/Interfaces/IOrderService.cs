using FoodSquad_API.Models.DTO.Order;

namespace FoodSquad_API.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDTO> CreateOrderAsync(OrderCreateDTO orderCreateDTO);
        Task<List<OrderDTO>> GetAllOrdersAsync(int page, int size);
        Task<List<OrderDTO>> GetOrdersByUserIdAsync(Guid userId, int page, int size);
        Task<OrderDTO> GetOrderByIdAsync(Guid id);
        Task<OrderDTO> UpdateOrderAsync(Guid id, OrderUpdateDTO orderUpdateDTO);
        Task<string> DeleteOrderAsync(Guid id); 
        Task<string> DeleteOrdersAsync(List<Guid> ids);
    }
}
