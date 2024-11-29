using FoodSquad_API.Models.DTO;

namespace FoodSquad_API.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDTO> CreateReviewAsync(ReviewDTO reviewDTO);
        Task<List<ReviewDTO>> GetReviewsByMenuItemIdAsync(long menuItemId);
        Task<List<ReviewDTO>> GetReviewsByUserIdAsync(Guid userId);
        Task<List<ReviewDTO>> GetAllReviewsAsync(int page, int size);
        Task<ReviewDTO> UpdateReviewAsync(long id, ReviewDTO reviewDTO);
        Task DeleteReviewAsync(long id);
    }
}
