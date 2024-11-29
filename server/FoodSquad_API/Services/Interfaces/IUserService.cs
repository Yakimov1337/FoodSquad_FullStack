using FoodSquad_API.Models.DTO;

namespace FoodSquad_API.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserResponseDTO>> GetAllUsersAsync(int page, int size);
        Task<UserResponseDTO> GetUserByIdAsync(Guid id);
        Task<UserResponseDTO> UpdateUserAsync(Guid id, UserUpdateDTO userUpdateDTO);
        Task DeleteUserAsync(Guid id);
    }
}
