using FoodSquad_API.Data;
using FoodSquad_API.Models.Entity;
using FoodSquad_API.Repositories.Interfaces;
using FoodSquad_API.Utilities;
using Microsoft.EntityFrameworkCore;

namespace FoodSquad_API.Repositories
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly MyDbContext _dbContext;

        public MenuItemRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MenuItem?> GetByIdAsync(long id)
        {
            return await _dbContext.MenuItems
                .Include(mi => mi.User)
                .FirstOrDefaultAsync(mi => mi.Id == id);
        }



        public async Task AddAsync(MenuItem menuItem)
        {
            await _dbContext.MenuItems.AddAsync(menuItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(MenuItem menuItem)
        {
            _dbContext.MenuItems.Update(menuItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(MenuItem menuItem)
        {
            _dbContext.MenuItems.Remove(menuItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<MenuItem> menuItems)
        {
            _dbContext.MenuItems.RemoveRange(menuItems);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetByIdsAsync(IEnumerable<long> ids)
        {
            return await _dbContext.MenuItems
                    .Include(mi => mi.User)
                .Where(mi => ids.Contains(mi.Id))
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<PaginatedList<MenuItem>> GetPagedItemsAsync(int page, int limit, string sortBy, bool desc, string categoryFilter, string isDefault, string priceSortDirection)
        {
            IQueryable<MenuItem> query = _dbContext.MenuItems;

            // Filter by category
            if (!string.IsNullOrEmpty(categoryFilter))
            {
                query = query.Where(mi => mi.Category.ToString() == categoryFilter);
            }

            // Filter by default status
            if (!string.IsNullOrEmpty(isDefault) && bool.TryParse(isDefault, out bool defaultStatus))
            {
                query = query.Where(mi => mi.DefaultItem == defaultStatus);
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                query = desc
                    ? query.OrderByDescending(e => EF.Property<object>(e, sortBy))
                    : query.OrderBy(e => EF.Property<object>(e, sortBy));
            }

            // Pagination
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip(page * limit)
                .Take(limit)
                .ToListAsync();

            return new PaginatedList<MenuItem>(items, totalCount, page, limit);

        }

        public async Task<IEnumerable<MenuItem>> GetByUserIdAsync(Guid userId)
        {
            return await _dbContext.MenuItems
                .Where(mi => mi.UserId == userId)
                .ToListAsync();
        }


    }
}
