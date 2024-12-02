using FoodSquad_API.Models.DTO.MenuItem;
using FoodSquad_API.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodSquad_API.Services.Interfaces
{
    public interface IMenuItemService
    {
        Task<MenuItemDTO> CreateMenuItemAsync(MenuItemCreateDTO menuItemCreateDTO);
        Task<MenuItemDTO> GetMenuItemByIdAsync(long id);
        Task<PaginatedResponseDTO<MenuItemDTO>> GetAllMenuItemsAsync(int page, int limit, string sortBy, bool desc, string categoryFilter, string isDefault, string priceSortDirection);
        Task<MenuItemDTO> UpdateMenuItemAsync(long id, MenuItemUpdateDTO menuItemUpdateDTO);
        Task<bool> DeleteMenuItemAsync(long id);
        Task<List<MenuItemDTO>> GetMenuItemsByIdsAsync(List<long> ids);
        Task<bool> DeleteMenuItemsByIdsAsync(List<long> ids);
    }
}
