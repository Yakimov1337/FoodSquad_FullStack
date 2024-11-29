using FoodSquad_API.Models.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    }
}
