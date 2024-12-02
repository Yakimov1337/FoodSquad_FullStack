using FoodSquad_API.Models.Entity;

namespace FoodSquad_API.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderWithDetailsAsync(Guid id);
        Task<IEnumerable<Order>> GetPagedOrdersAsync(int page, int size);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId, int page, int size);
        Task<IEnumerable<Order>> GetOrdersByIdsAsync(List<Guid> ids);
        Task<long> CountOrdersByUserIdAsync(Guid userId);
        Task<int?> SumQuantityByMenuItemIdAsync(long menuItemId); 
        Task RemoveMenuItemReferencesAsync(long menuItemId); 
        Task CreateOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(Guid id);
        Task DeleteOrdersAsync(List<Guid> ids);
        Task DeleteRangeAsync(IEnumerable<Order> orders);
    }
}
