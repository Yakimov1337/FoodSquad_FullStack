using AutoMapper;
using FoodSquad_API.Models.DTO;
using FoodSquad_API.Models.DTO.MenuItem;
using FoodSquad_API.Models.Entity;
using FoodSquad_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace FoodSquad_API.Controllers
{
    [ApiController]
    [Route("api/menu-items")]
    [Authorize] // Ensure all methods require authentication by default
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;
        private readonly IMapper _mapper;

        public MenuItemController(IMenuItemService menuItemService, IMapper mapper)
        {
            _menuItemService = menuItemService;
            _mapper = mapper;
            _mapper = mapper;
        }

        [Authorize(Policy = "ModeratorPolicy")]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new menu item",
            Description = "Allows MODERATOR and ADMIN to create a new menu item with the provided details."
        )]
        [SwaggerRequestExample(typeof(MenuItemCreateDTO), typeof(MenuItemCreateDTOExample))]
        public async Task<IActionResult> CreateMenuItem([FromBody] MenuItemCreateDTO menuItemCreateDTO)
        {
            // Directly call the service with the MenuItemCreateDTO
            var createdMenuItem = await _menuItemService.CreateMenuItemAsync(menuItemCreateDTO);
            return Ok(createdMenuItem);
        }


        [Authorize(Policy = "UserPolicy")]
        [HttpGet("{id}")]
        [SwaggerOperation(
    Summary = "Get menu item by ID",
    Description = "Allows NORMAL, MODERATOR, and ADMIN to retrieve a menu item by its ID."
)]
        public async Task<IActionResult> GetMenuItemById(long id)
        {
            var result = await _menuItemService.GetMenuItemByIdAsync(id);
            if (result == null)
                return NotFound($"MenuItem with ID {id} not found.");
            return Ok(result);
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpGet]
        [SwaggerOperation(
    Summary = "Get all menu items",
    Description = "Allows NORMAL, MODERATOR, and ADMIN to retrieve all menu items with optional filters for pagination, sorting, and filtering by category or default status."
)]
        public async Task<IActionResult> GetAllMenuItems(
            [FromQuery] int page = 0,
            [FromQuery] int limit = 10,
            [FromQuery] string sortBy = null,
            [FromQuery] bool desc = false,
            [FromQuery] string categoryFilter = null,
            [FromQuery] string isDefault = null,
            [FromQuery] string priceSortDirection = null)
        {
            var result = await _menuItemService.GetAllMenuItemsAsync(page, limit, sortBy, desc, categoryFilter, isDefault, priceSortDirection);
            return Ok(result);
        }

        [Authorize(Policy = "ModeratorPolicy")]
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Update an existing menu item",
            Description = "Allows MODERATOR and ADMIN to update an existing menu item by its ID."
        )]
        public async Task<IActionResult> UpdateMenuItem(long id, [FromBody] MenuItemUpdateDTO menuItemUpdateDTO)
        {
            var updatedMenuItem = await _menuItemService.UpdateMenuItemAsync(id, menuItemUpdateDTO);
            return Ok(updatedMenuItem);
        }


        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        [SwaggerOperation(
    Summary = "Delete a menu item",
    Description = "Allows ADMIN to delete a menu item by its ID."
)]
        public async Task<IActionResult> DeleteMenuItem(long id)
        {
            var result = await _menuItemService.DeleteMenuItemAsync(id);
            if (!result)
                return NotFound($"MenuItem with ID {id} not found.");
            return Ok(new { message = $"MenuItem with ID {id} deleted successfully." });
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpGet("batch")]
        [SwaggerOperation(
    Summary = "Get multiple menu items by IDs",
    Description = "Allows NORMAL, MODERATOR, and ADMIN to retrieve multiple menu items by their IDs."
)]
        public async Task<IActionResult> GetMenuItemsByIds([FromQuery] List<long> ids)
        {
            var result = await _menuItemService.GetMenuItemsByIdsAsync(ids);
            return Ok(result);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("batch")]
        [SwaggerOperation(
    Summary = "Delete multiple menu items",
    Description = "Allows ADMIN to delete multiple menu items by their IDs."
)]
        public async Task<IActionResult> DeleteMenuItemsByIds([FromQuery] List<long> ids)
        {
            var result = await _menuItemService.DeleteMenuItemsByIdsAsync(ids);
            return Ok(new { message = $"MenuItems with IDs {string.Join(", ", ids)} deleted successfully." });
        }
    }
}
