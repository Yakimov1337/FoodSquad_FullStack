using AutoMapper;
using FoodSquad_API.Models.DTO.Review;
using FoodSquad_API.Models.Entity;
using FoodSquad_API.Repositories.Interfaces;
using FoodSquad_API.Services;
using FoodSquad_API.Services.Interfaces;
using Moq;
using Xunit;

namespace FoodSquad_API.Tests.Services
{
    public class ReviewServiceTests
    {
        private readonly Mock<IReviewRepository> _reviewRepositoryMock;
        private readonly Mock<IMenuItemRepository> _menuItemRepositoryMock;
        private readonly Mock<IUserContextService> _userContextServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ReviewService _reviewService;

        public ReviewServiceTests()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _menuItemRepositoryMock = new Mock<IMenuItemRepository>();
            _userContextServiceMock = new Mock<IUserContextService>();
            _mapperMock = new Mock<IMapper>();

            _reviewService = new ReviewService(
                _reviewRepositoryMock.Object,
                _menuItemRepositoryMock.Object,
                _userContextServiceMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task CreateReviewAsync_ShouldCreateReview_WhenInputIsValid()
        {
            // Arrange
            var menuItemId = 1L;
            var userId = Guid.NewGuid();

            var reviewCreateDTO = new ReviewCreateDTO
            {
                MenuItemId = menuItemId,
                Rating = 5,
                Comment = "Great!"
            };

            var currentUser = new User { Id = userId, Email = "test@test.com" };
            var menuItem = new MenuItem { Id = menuItemId, Title = "Test Menu Item" };
            var review = new Review { Id = 1, Rating = 5, Comment = "Great!" };
            var reviewDTO = new ReviewDTO { Id = 1, Rating = 5, Comment = "Great!" };

            _menuItemRepositoryMock.Setup(x => x.GetByIdAsync(menuItemId)).ReturnsAsync(menuItem);
            _userContextServiceMock.Setup(x => x.GetCurrentUserAsync()).ReturnsAsync(currentUser);
            _mapperMock.Setup(x => x.Map<Review>(reviewCreateDTO)).Returns(review);
            _reviewRepositoryMock.Setup(x => x.AddAsync(review)).Returns(Task.CompletedTask);
            _mapperMock.Setup(x => x.Map<ReviewDTO>(review)).Returns(reviewDTO);

            // Act
            var result = await _reviewService.CreateReviewAsync(reviewCreateDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reviewDTO.Id, result.Id);
            Assert.Equal(reviewDTO.Rating, result.Rating);

            _menuItemRepositoryMock.Verify(x => x.GetByIdAsync(menuItemId), Times.Once);
            _userContextServiceMock.Verify(x => x.GetCurrentUserAsync(), Times.Once);
            _reviewRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Review>()), Times.Once);
        }

        [Fact]
        public async Task GetReviewsByMenuItemIdAsync_ShouldReturnReviews_WhenMenuItemHasReviews()
        {
            // Arrange
            var menuItemId = 1L;
            var reviews = new List<Review>
            {
                new Review { Id = 1, Rating = 5, Comment = "Great!" },
                new Review { Id = 2, Rating = 4, Comment = "Good!" }
            };

            var reviewDTOs = new List<ReviewDTO>
            {
                new ReviewDTO { Id = 1, Rating = 5, Comment = "Great!" },
                new ReviewDTO { Id = 2, Rating = 4, Comment = "Good!" }
            };

            _reviewRepositoryMock.Setup(x => x.GetByMenuItemIdAsync(menuItemId)).ReturnsAsync(reviews);
            _mapperMock.Setup(x => x.Map<List<ReviewDTO>>(reviews)).Returns(reviewDTOs);

            // Act
            var result = await _reviewService.GetReviewsByMenuItemIdAsync(menuItemId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reviewDTOs.Count, result.Count);

            _reviewRepositoryMock.Verify(x => x.GetByMenuItemIdAsync(menuItemId), Times.Once);
        }

        [Fact]
        public async Task UpdateReviewAsync_ShouldUpdateReview_WhenReviewExists()
        {
            // Arrange
            var reviewId = 1L;
            var reviewUpdateDTO = new ReviewUpdateDTO { Rating = 4, Comment = "Updated Comment" };

            var existingReview = new Review { Id = reviewId, Rating = 5, Comment = "Old Comment" };
            var updatedReview = new Review { Id = reviewId, Rating = 4, Comment = "Updated Comment" };
            var updatedReviewDTO = new ReviewDTO { Id = reviewId, Rating = 4, Comment = "Updated Comment" };

            _reviewRepositoryMock.Setup(x => x.GetByIdAsync(reviewId)).ReturnsAsync(existingReview);
            _userContextServiceMock.Setup(x => x.CheckOwnershipAsync(existingReview.User)).Returns(Task.CompletedTask);
            _mapperMock.Setup(x => x.Map(reviewUpdateDTO, existingReview)).Returns(updatedReview);
            _reviewRepositoryMock.Setup(x => x.UpdateAsync(existingReview)).Returns(Task.CompletedTask);
            _mapperMock.Setup(x => x.Map<ReviewDTO>(existingReview)).Returns(updatedReviewDTO);

            // Act
            var result = await _reviewService.UpdateReviewAsync(reviewId, reviewUpdateDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedReviewDTO.Id, result.Id);
            Assert.Equal(updatedReviewDTO.Comment, result.Comment);

            _reviewRepositoryMock.Verify(x => x.GetByIdAsync(reviewId), Times.Once);
            _reviewRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Review>()), Times.Once);
        }

        [Fact]
        public async Task DeleteReviewAsync_ShouldDeleteReview_WhenReviewExists()
        {
            // Arrange
            var reviewId = 1L;
            var review = new Review { Id = reviewId, User = new User { Id = Guid.NewGuid() } };

            _reviewRepositoryMock.Setup(x => x.GetByIdAsync(reviewId)).ReturnsAsync(review);
            _userContextServiceMock.Setup(x => x.CheckOwnershipAsync(review.User)).Returns(Task.CompletedTask);
            _reviewRepositoryMock.Setup(x => x.DeleteAsync(review)).Returns(Task.CompletedTask);

            // Act
            await _reviewService.DeleteReviewAsync(reviewId);

            // Assert
            _reviewRepositoryMock.Verify(x => x.GetByIdAsync(reviewId), Times.Once);
            _reviewRepositoryMock.Verify(x => x.DeleteAsync(review), Times.Once);
        }
    }
}
