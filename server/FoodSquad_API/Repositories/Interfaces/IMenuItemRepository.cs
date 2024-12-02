using FoodSquad_API.Models.Entity;
using FoodSquad_API.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodSquad_API.Repositories.Interfaces
{
    public interface IMenuItemRepository
    {
        Task<MenuItem> GetByIdAsync(long id);
        Task<IEnumerable<MenuItem>> GetByUserIdAsync(Guid userId);
        Task AddAsync(MenuItem menuItem);
        Task UpdateAsync(MenuItem menuItem);
        Task DeleteAsync(MenuItem menuItem);
        Task SaveChangesAsync();
        Task DeleteRangeAsync(IEnumerable<MenuItem> menuItems);
        Task<IEnumerable<MenuItem>> GetByIdsAsync(IEnumerable<long> ids);
        Task<PaginatedList<MenuItem>> GetPagedItemsAsync(int page, int limit, string sortBy, bool desc, string categoryFilter, string isDefault, string priceSortDirection);


    }
}
