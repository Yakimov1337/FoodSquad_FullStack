﻿using FoodSquad_API.Models.Entity;
using FoodSquad_API.Models.Enums;
using FoodSquad_API.Repositories.Interfaces;
using FoodSquad_API.Services.Interfaces;
using System.Security.Claims;

namespace FoodSquad_API.Services
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;

        public UserContextService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            var email = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                throw new UnauthorizedAccessException("User is not logged in.");

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new UnauthorizedAccessException("Current user not found in the database.");

            // Set the role explicitly as uppercase
            user.Role = Enum.Parse<UserRole>(user.Role.ToString(), true);

            return user;
        }

        public async Task CheckOwnershipAsync(User resourceOwner)
        {
            if (resourceOwner == null)
            {
                throw new ArgumentNullException(nameof(resourceOwner), "The resource owner is null.");
            }

            var currentUser = await GetCurrentUserAsync();

            // Log debug information to verify the current user and resource owner
            Console.WriteLine($"Current User: Id={currentUser.Id}, Email={currentUser.Email}, Role={currentUser.Role}");
            Console.WriteLine($"Resource Owner: Id={resourceOwner.Id}, Email={resourceOwner.Email}, Role={resourceOwner.Role}");


            if (currentUser.Id != resourceOwner.Id &&
                currentUser.Role != UserRole.Admin &&
                currentUser.Role != UserRole.Moderator)
            {
                throw new UnauthorizedAccessException("Access denied.");
            }
        }

    }
}
