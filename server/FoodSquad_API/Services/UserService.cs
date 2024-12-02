using FoodSquad_API.Models.Entity;
using FoodSquad_API.Models.Enums;
using FoodSquad_API.Repositories.Interfaces;
using FoodSquad_API.Services.Interfaces;
using AutoMapper;
using FoodSquad_API.Models.DTO.User;
using FoodSquad_API.Repositories;
using FoodSquad_API.Data;

namespace FoodSquad_API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;
        private readonly MyDbContext _dbContext;

        public UserService(
            IUserRepository userRepository,
            IOrderRepository orderRepository,
            IReviewRepository reviewRepository,
            IMenuItemRepository menuItemRepository,
            IUserContextService userContextService,
            IMapper mapper,
            MyDbContext dbContext)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _reviewRepository = reviewRepository;
            _menuItemRepository = menuItemRepository;
            _userContextService = userContextService;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        // Get all users (Paginated)
        public async Task<List<UserResponseDTO>> GetAllUsersAsync(int page, int size)
        {
            var users = await _userRepository.GetAllPaginatedAsync(page, size);

            var userDTOs = new List<UserResponseDTO>();
            foreach (var user in users)
            {
                var ordersCount = await _orderRepository.CountOrdersByUserIdAsync(user.Id);
                var userDTO = _mapper.Map<UserResponseDTO>(user);
                userDTO.OrdersCount = ordersCount;
                userDTOs.Add(userDTO);
            }

            return userDTOs;
        }

        // Get user by ID
        public async Task<UserResponseDTO> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            // Check ownership or admin/mod access
            await _userContextService.CheckOwnershipAsync(user);

            var ordersCount = await _orderRepository.CountOrdersByUserIdAsync(user.Id);
            var userDTO = _mapper.Map<UserResponseDTO>(user);
            userDTO.OrdersCount = ordersCount;
            return userDTO;
        }

        // Update user
        public async Task<UserResponseDTO> UpdateUserAsync(Guid id, UserUpdateDTO userUpdateDTO)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            // Check ownership or admin/mod access
            await _userContextService.CheckOwnershipAsync(user);

            // Business logic for role updates
            var currentUser = await _userContextService.GetCurrentUserAsync();
            if (currentUser.Id == id && currentUser.Role.ToString() != userUpdateDTO.Role)
                throw new InvalidOperationException("Users cannot update their own role.");

            if (currentUser.Role == UserRole.Normal && userUpdateDTO.Role != UserRole.Normal.ToString())
                throw new InvalidOperationException("Normal users cannot change roles.");

            if (currentUser.Role != UserRole.Admin && userUpdateDTO.Role == UserRole.Admin.ToString())
                throw new InvalidOperationException("Only admin users can assign the admin role.");

            if (user.Role == UserRole.Admin && userUpdateDTO.Role != UserRole.Admin.ToString())
                throw new InvalidOperationException("Admin user role cannot be changed.");

            // Update user details
            user.Name = userUpdateDTO.Name;
            user.Role = Enum.Parse<UserRole>(userUpdateDTO.Role);
            user.ImageUrl = userUpdateDTO.ImageUrl;
            user.PhoneNumber = userUpdateDTO.PhoneNumber;

            await _userRepository.UpdateAsync(user);

            var ordersCount = await _orderRepository.CountOrdersByUserIdAsync(user.Id);
            var updatedUserDTO = _mapper.Map<UserResponseDTO>(user);
            updatedUserDTO.OrdersCount = ordersCount;
            return updatedUserDTO;
        }

        // Delete user
        public async Task DeleteUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            // Prevent admin users from being deleted
            if (user.Role == UserRole.Admin)
                throw new InvalidOperationException("Admin users cannot be deleted.");

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Delete dependent entities manually
                // Delete user's reviews
                var reviews = await _reviewRepository.GetByUserIdAsync(userId);
                if (reviews.Any())
                {
                    await _reviewRepository.DeleteRangeAsync(reviews);
                }

                // Delete user's orders
                var orders = await _orderRepository.GetOrdersByUserIdAsync(userId, 0, int.MaxValue); // No pagination for deletion
                if (orders.Any())
                {
                    await _orderRepository.DeleteRangeAsync(orders);
                }

                // Delete user's menu items
                var menuItems = await _menuItemRepository.GetByUserIdAsync(userId);
                if (menuItems.Any())
                {
                    foreach (var menuItem in menuItems)
                    {
                        // Remove references in OrderMenuItems table
                        await _orderRepository.RemoveMenuItemReferencesAsync(menuItem.Id);
                    }

                    await _menuItemRepository.DeleteRangeAsync(menuItems);
                }

                // Finally, delete the user
                await _userRepository.DeleteAsync(user);

                // Commit the transaction
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("An error occurred while deleting the user.", ex);
            }
        }

    }
}
