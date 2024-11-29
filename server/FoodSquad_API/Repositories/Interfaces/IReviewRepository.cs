using FoodSquad_API.Models.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodSquad_API.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review> GetByIdAsync(long id);
        Task<List<Review>> GetByMenuItemIdAsync(long menuItemId);
        Task<List<Review>> GetByUserIdAsync(Guid userId);
        Task<long> CountByMenuItemIdAsync(long menuItemId);
        Task<double?> FindAverageRatingByMenuItemIdAsync(long menuItemId); 
        Task<List<Review>> GetAllReviewsAsync(int page, int size);
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(Review review);
    }
}
