using FoodSquad_API.Data;
using FoodSquad_API.Models.Entity;
using FoodSquad_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodSquad_API.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly MyDbContext _dbContext;

        public OrderRepository(MyDbContext context)
        {
            _dbContext = context;
        }

        public async Task<Order> GetOrderWithDetailsAsync(Guid id)
        {
            return await _dbContext.Orders
                .Include(o => o.MenuItemsWithQuantity)
                .ThenInclude(miq => miq.MenuItem)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetPagedOrdersAsync(int page, int size)
        {
            return await _dbContext.Orders
                .Include(o => o.MenuItemsWithQuantity)
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedOn)
                .Skip(page * size)
                .Take(size)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId, int page, int size)
        {
            return await _dbContext.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.MenuItemsWithQuantity)
                .OrderByDescending(o => o.CreatedOn)
                .Skip(page * size)
                .Take(size)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByIdsAsync(List<Guid> ids)
        {
            return await _dbContext.Orders
                .Where(o => ids.Contains(o.Id))
                .Include(o => o.User) // Include the User property
                .ToListAsync();
        }

        public async Task<long> CountOrdersByUserIdAsync(Guid userId)
        {
            return await _dbContext.Orders.CountAsync(o => o.UserId == userId);
        }

        public async Task<int?> SumQuantityByMenuItemIdAsync(long menuItemId)
        {
            return await _dbContext.OrderMenuItems
                .Where(omi => omi.MenuItemId == menuItemId)
                .SumAsync(omi => (int?)omi.Quantity); 
        }

        public async Task RemoveMenuItemReferencesAsync(long menuItemId)
        {
            var references = await _dbContext.OrderMenuItems
                .Where(omi => omi.MenuItemId == menuItemId)
                .ToListAsync();

            if (references.Any())
            {
                _dbContext.OrderMenuItems.RemoveRange(references);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task CreateOrderAsync(Order order)
        {
            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(Guid id)
        {
            var order = await _dbContext.Orders.FindAsync(id);
            if (order != null)
            {
                _dbContext.Orders.Remove(order);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteOrdersAsync(List<Guid> ids)
        {
            var orders = await _dbContext.Orders.Where(o => ids.Contains(o.Id)).ToListAsync();
            _dbContext.Orders.RemoveRange(orders);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<Order> orders)
        {
            _dbContext.Orders.RemoveRange(orders);
            await _dbContext.SaveChangesAsync();
        }


    }
}
