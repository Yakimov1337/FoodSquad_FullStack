using FoodSquad_API.Models.DTO.Review;
using FoodSquad_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FoodSquad_API.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    [Authorize] // Ensure all actions require authentication by default
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a review", Description = "Allows NORMAL, MODERATOR, and ADMIN to create a review.")]
        public async Task<IActionResult> CreateReview([FromBody] ReviewCreateDTO reviewCreateDTO)
        {
            var result = await _reviewService.CreateReviewAsync(reviewCreateDTO);
            return CreatedAtAction(nameof(CreateReview), new { id = result.Id }, result);
        }


        [Authorize(Policy = "UserPolicy")]
        [HttpGet("menu-item/{menuItemId}")]
        [SwaggerOperation(
    Summary = "Get reviews by menu item ID",
    Description = "Allows NORMAL, MODERATOR, and ADMIN to retrieve all reviews for a specific menu item."
)]
        public async Task<IActionResult> GetReviewsByMenuItemId(long menuItemId)
        {
            var reviews = await _reviewService.GetReviewsByMenuItemIdAsync(menuItemId);
            return Ok(reviews);
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpGet("user/{userId}")]
        [SwaggerOperation(
    Summary = "Get reviews by user ID",
    Description = "Allows NORMAL, MODERATOR, and ADMIN to retrieve all reviews created by a specific user."
)]
        public async Task<IActionResult> GetReviewsByUserId(Guid userId)
        {
            var reviews = await _reviewService.GetReviewsByUserIdAsync(userId);
            return Ok(reviews);
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpGet]
        [SwaggerOperation(
    Summary = "Get all reviews",
    Description = "Allows NORMAL, MODERATOR, and ADMIN to view all reviews with optional pagination."
)]
        public async Task<IActionResult> GetAllReviews([FromQuery] int page = 0, [FromQuery] int size = 10)
        {
            var reviews = await _reviewService.GetAllReviewsAsync(page, size);
            return Ok(reviews);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update a review", Description = "Allows MODERATOR and ADMIN to update an existing review.")]
        public async Task<IActionResult> UpdateReview(long id, [FromBody] ReviewUpdateDTO reviewUpdateDTO)
        {
            var result = await _reviewService.UpdateReviewAsync(id, reviewUpdateDTO);
            return Ok(result);
        }


        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        [SwaggerOperation(
    Summary = "Delete a review",
    Description = "Allows ADMIN to delete a review by its ID."
)]
        public async Task<IActionResult> DeleteReview(long id)
        {
            await _reviewService.DeleteReviewAsync(id);
            return NoContent();
        }
    }
}
