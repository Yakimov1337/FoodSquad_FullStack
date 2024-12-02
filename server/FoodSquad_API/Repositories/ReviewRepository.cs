using FoodSquad_API.Data;
using FoodSquad_API.Models.Entity;
using FoodSquad_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodSquad_API.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly MyDbContext _dbContext;

        public ReviewRepository(MyDbContext context)
        {
            _dbContext = context;
        }

        // Fetch a review by ID with the associated User entity
        public async Task<Review> GetByIdAsync(long id)
        {
            return await _dbContext.Reviews
                .Include(r => r.User) // Include User entity
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // Fetch reviews by MenuItem ID, include User for email and image
        public async Task<List<Review>> GetByMenuItemIdAsync(long menuItemId)
        {
            return await _dbContext.Reviews
                .Where(r => r.MenuItemId == menuItemId)
                .Include(r => r.User) // Include User entity
                .ToListAsync();
        }

        // Fetch reviews by User ID, include User for email and image
        public async Task<List<Review>> GetByUserIdAsync(Guid userId)
        {
            return await _dbContext.Reviews
                .Where(r => r.UserId == userId)
                .Include(r => r.User) // Include User entity
                .ToListAsync();
        }

        // Count reviews by MenuItem ID
        public async Task<long> CountByMenuItemIdAsync(long menuItemId)
        {
            return await _dbContext.Reviews
                .Where(r => r.MenuItemId == menuItemId)
                .LongCountAsync();
        }

        // Calculate the average rating for a MenuItem
        public async Task<double?> FindAverageRatingByMenuItemIdAsync(long menuItemId)
        {
            return await _dbContext.Reviews
                .Where(r => r.MenuItemId == menuItemId)
                .AverageAsync(r => (double?)r.Rating); // Return null if no entries exist
        }

        // Fetch all reviews with pagination and include User for email and image
        public async Task<List<Review>> GetAllReviewsAsync(int page, int size)
        {
            return await _dbContext.Reviews
                .Include(r => r.User) // Include User entity
                .OrderByDescending(r => r.CreatedOn)
                .Skip(page * size)
                .Take(size)
                .ToListAsync();
        }

        // Add a new review to the database
        public async Task AddAsync(Review review)
        {
            await _dbContext.Reviews.AddAsync(review);
            await _dbContext.SaveChangesAsync();
        }

        // Update an existing review
        public async Task UpdateAsync(Review review)
        {
            _dbContext.Reviews.Update(review);
            await _dbContext.SaveChangesAsync();
        }

        // Delete a review
        public async Task DeleteAsync(Review review)
        {
            _dbContext.Reviews.Remove(review);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<Review> reviews)
        {
            _dbContext.Reviews.RemoveRange(reviews);
            await _dbContext.SaveChangesAsync();
        }

    }
}
