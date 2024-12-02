using AutoMapper;
using FoodSquad_API.Models.DTO.Review;
using FoodSquad_API.Models.Entity;
using FoodSquad_API.Repositories.Interfaces;
using FoodSquad_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodSquad_API.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;

        public ReviewService(
            IReviewRepository reviewRepository,
            IMenuItemRepository menuItemRepository,
            IUserContextService userContextService,
            IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _menuItemRepository = menuItemRepository;
            _userContextService = userContextService;
            _mapper = mapper;
        }

        public async Task<ReviewDTO> CreateReviewAsync(ReviewCreateDTO reviewCreateDTO)
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(reviewCreateDTO.MenuItemId);
            if (menuItem == null)
                throw new KeyNotFoundException("Menu item not found");

            var currentUser = await _userContextService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                throw new InvalidOperationException("No current user found in the context.");
            }
            Console.WriteLine($"Current User: {currentUser.Id} - {currentUser.Email}");

            var review = _mapper.Map<Review>(reviewCreateDTO);
            review.MenuItem = menuItem;
            review.User = currentUser;

            await _reviewRepository.AddAsync(review);

            return _mapper.Map<ReviewDTO>(review);
        }

        public async Task<List<ReviewDTO>> GetReviewsByMenuItemIdAsync(long menuItemId)
        {
            var reviews = await _reviewRepository.GetByMenuItemIdAsync(menuItemId);
            return _mapper.Map<List<ReviewDTO>>(reviews);
        }

        public async Task<List<ReviewDTO>> GetReviewsByUserIdAsync(Guid userId)
        {
            var reviews = await _reviewRepository.GetByUserIdAsync(userId);
            return _mapper.Map<List<ReviewDTO>>(reviews);
        }

        public async Task<List<ReviewDTO>> GetAllReviewsAsync(int page, int size)
        {
            var reviews = await _reviewRepository.GetAllReviewsAsync(page, size);
            return _mapper.Map<List<ReviewDTO>>(reviews);
        }

        public async Task<ReviewDTO> UpdateReviewAsync(long id, ReviewUpdateDTO reviewUpdateDTO)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                throw new KeyNotFoundException("Review not found");

            await _userContextService.CheckOwnershipAsync(review.User);

            _mapper.Map(reviewUpdateDTO, review);
            await _reviewRepository.UpdateAsync(review);

            return _mapper.Map<ReviewDTO>(review);
        }

        public async Task DeleteReviewAsync(long id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                throw new KeyNotFoundException("Review not found");

            await _userContextService.CheckOwnershipAsync(review.User);

            await _reviewRepository.DeleteAsync(review);
        }
    }
}
