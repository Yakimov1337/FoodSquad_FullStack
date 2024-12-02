using FoodSquad_API.Models.DTO.Order;
using FoodSquad_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FoodSquad_API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize] // Ensure all actions require authentication by default
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new order",
            Description = "Allows NORMAL, MODERATOR, and ADMIN to create a new order with the provided details."
        )]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDTO orderCreateDTO)
        {
            var createdOrder = await _orderService.CreateOrderAsync(orderCreateDTO);
            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(
    Summary = "Update an order",
    Description = "Allows NORMAL, MODERATOR, and ADMIN to update an existing order."
)]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] OrderUpdateDTO orderUpdateDTO)
        {
            var updatedOrder = await _orderService.UpdateOrderAsync(id, orderUpdateDTO);
            return Ok(updatedOrder);
        }



        [Authorize(Policy = "UserPolicy")]
        [HttpGet("{id}")]
        [SwaggerOperation(
    Summary = "Get order by ID",
    Description = "Allows NORMAL, MODERATOR, and ADMIN to retrieve their own order by its ID."
)]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return Ok(order);
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpGet("user/{userId}")]
        [SwaggerOperation(
    Summary = "Get orders by user ID",
    Description = "Allows NORMAL, MODERATOR, and ADMIN to retrieve all orders for a specific user with pagination."
)]
        public async Task<IActionResult> GetOrdersByUserId(Guid userId, [FromQuery] int page = 0, [FromQuery] int size = 10)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId, page, size);
            return Ok(orders);
        }

        [Authorize(Policy = "ModeratorPolicy")]
        [HttpGet]
        [SwaggerOperation(
    Summary = "Get paginated orders",
    Description = "Allows MODERATOR and ADMIN to view all orders with pagination."
)]
        public async Task<IActionResult> GetPagedOrders([FromQuery] int page = 0, [FromQuery] int size = 10)
        {
            var orders = await _orderService.GetAllOrdersAsync(page, size);
            return Ok(orders);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete an order by ID",
            Description = "Allows ADMIN to delete an order by its ID."
        )]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var response = await _orderService.DeleteOrderAsync(id);
            return Ok(new { message = response });
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("batch")]
        [SwaggerOperation(
           Summary = "Delete multiple orders",
           Description = "Allows ADMIN to delete multiple orders by their IDs."
       )]
        public async Task<IActionResult> DeleteOrders([FromQuery] List<Guid> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest(new { message = "The list of IDs cannot be null or empty." });
            }

            var response = await _orderService.DeleteOrdersAsync(ids);
            return Ok(new { message = $"{ids.Count} orders successfully deleted." });
        }




    }
}
