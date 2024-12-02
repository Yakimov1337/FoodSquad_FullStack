using AutoMapper;
using FoodSquad_API.Models.DTO;
using FoodSquad_API.Models.DTO.MenuItem;
using FoodSquad_API.Models.Entity;
using FoodSquad_API.Repositories.Interfaces;
using FoodSquad_API.Services.Interfaces;

namespace FoodSquad_API.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;

        public MenuItemService(
            IMenuItemRepository menuItemRepository,
            IOrderRepository orderRepository,
            IReviewRepository reviewRepository,
            IUserContextService userContextService,
            IMapper mapper)
        {
            _menuItemRepository = menuItemRepository;
            _orderRepository = orderRepository;
            _reviewRepository = reviewRepository;
            _userContextService = userContextService;
            _mapper = mapper;
        }

        public async Task<MenuItemDTO> CreateMenuItemAsync(MenuItemCreateDTO menuItemCreateDTO)
        {
            var currentUser = await _userContextService.GetCurrentUserAsync();

            // Map the DTO directly to the MenuItem entity
            var menuItem = _mapper.Map<MenuItem>(menuItemCreateDTO);
            menuItem.User = currentUser; // Set the current user as the owner

            if (menuItem.DefaultItem == null)
                menuItem.DefaultItem = false; // Ensure default is set to false

            await _menuItemRepository.AddAsync(menuItem);
            await _menuItemRepository.SaveChangesAsync();

            // Map the saved entity back to a MenuItemDTO for the response
            return _mapper.Map<MenuItemDTO>(menuItem);
        }


        public async Task<MenuItemDTO> GetMenuItemByIdAsync(long id)
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(id);
            if (menuItem == null)
                throw new KeyNotFoundException($"MenuItem with ID {id} not found.");

            await _userContextService.CheckOwnershipAsync(menuItem.User);

            var salesCount = await _orderRepository.SumQuantityByMenuItemIdAsync(id) ?? 0;
            var reviewCount = await _reviewRepository.CountByMenuItemIdAsync(id);
            var averageRating = Math.Round(await _reviewRepository.FindAverageRatingByMenuItemIdAsync(id) ?? 0.0, 1);

            return new MenuItemDTO(menuItem, salesCount, reviewCount, averageRating);
        }

        public async Task<PaginatedResponseDTO<MenuItemDTO>> GetAllMenuItemsAsync(int page, int limit, string sortBy, bool desc, string categoryFilter, string isDefault, string priceSortDirection)
        {
            var paginatedItems = await _menuItemRepository.GetPagedItemsAsync(page, limit, sortBy, desc, categoryFilter, isDefault, priceSortDirection);
            var menuItemsDTOs = paginatedItems.Items.Select(item =>
            {
                var salesCount = _orderRepository.SumQuantityByMenuItemIdAsync(item.Id).Result ?? 0;
                var reviewCount = _reviewRepository.CountByMenuItemIdAsync(item.Id).Result;
                var averageRating = Math.Round(_reviewRepository.FindAverageRatingByMenuItemIdAsync(item.Id).Result ?? 0.0, 1);
                return new MenuItemDTO(item, salesCount, reviewCount, averageRating);
            }).ToList();

            return new PaginatedResponseDTO<MenuItemDTO>(menuItemsDTOs, paginatedItems.TotalCount);
        }

        public async Task<MenuItemDTO> UpdateMenuItemAsync(long id, MenuItemUpdateDTO menuItemUpdateDTO)
        {
            var existingMenuItem = await _menuItemRepository.GetByIdAsync(id);
            if (existingMenuItem == null)
                throw new KeyNotFoundException($"MenuItem with ID {id} not found.");

            await _userContextService.CheckOwnershipAsync(existingMenuItem.User);

            // Map the DTO properties to the existing entity
            _mapper.Map(menuItemUpdateDTO, existingMenuItem);

            // Ensure defaultItem is not null
            if (existingMenuItem.DefaultItem == null)
                existingMenuItem.DefaultItem = false;

            await _menuItemRepository.UpdateAsync(existingMenuItem);
            await _menuItemRepository.SaveChangesAsync();

            return _mapper.Map<MenuItemDTO>(existingMenuItem);
        }

        public async Task<List<MenuItemDTO>> GetMenuItemsByIdsAsync(List<long> ids)
        {
            var menuItems = await _menuItemRepository.GetByIdsAsync(ids);
            if (!menuItems.Any())
                throw new KeyNotFoundException("No MenuItems found for the given IDs.");

            return menuItems.Select(menuItem =>
            {
                var salesCount = _orderRepository.SumQuantityByMenuItemIdAsync(menuItem.Id).Result ?? 0;
                var reviewCount = _reviewRepository.CountByMenuItemIdAsync(menuItem.Id).Result;
                var averageRating = Math.Round(_reviewRepository.FindAverageRatingByMenuItemIdAsync(menuItem.Id).Result ?? 0.0, 1);
                return new MenuItemDTO(menuItem, salesCount, reviewCount, averageRating);
            }).ToList();
        }

        public async Task<bool> DeleteMenuItemAsync(long id)
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(id);
            if (menuItem == null)
            {
                throw new KeyNotFoundException($"MenuItem with ID {id} not found.");
            }

            await _userContextService.CheckOwnershipAsync(menuItem.User);

            // Remove references in orders to avoid foreign key conflicts
            await _orderRepository.RemoveMenuItemReferencesAsync(menuItem.Id);

            await _menuItemRepository.DeleteAsync(menuItem);
            return true;
        }



        public async Task<bool> DeleteMenuItemsByIdsAsync(List<long> ids)
        {
            var menuItems = await _menuItemRepository.GetByIdsAsync(ids);
            if (!menuItems.Any())
                throw new KeyNotFoundException("No MenuItems found for the given IDs.");

            foreach (var menuItem in menuItems)
            {
                await _userContextService.CheckOwnershipAsync(menuItem.User);
                await _orderRepository.RemoveMenuItemReferencesAsync(menuItem.Id);
            }

            await _menuItemRepository.DeleteRangeAsync(menuItems);
            await _menuItemRepository.SaveChangesAsync();

            return true;
        }
    }
}
