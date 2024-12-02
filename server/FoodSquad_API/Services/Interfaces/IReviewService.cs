using FoodSquad_API.Models.DTO.Review;

namespace FoodSquad_API.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDTO> CreateReviewAsync(ReviewCreateDTO reviewCreateDTO);
        Task<List<ReviewDTO>> GetReviewsByMenuItemIdAsync(long menuItemId);
        Task<List<ReviewDTO>> GetReviewsByUserIdAsync(Guid userId);
        Task<List<ReviewDTO>> GetAllReviewsAsync(int page, int size);
        Task<ReviewDTO> UpdateReviewAsync(long id, ReviewUpdateDTO reviewUpdateDTO);
        Task DeleteReviewAsync(long id);
    }
}
