using FoodSquad_API.Models.DTO;
using FoodSquad_API.Models.Entity;
using System.Threading.Tasks;

namespace FoodSquad_API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponseDTO> RegisterUserAsync(UserRegistrationDTO userRegistrationDTO);
        Task<UserResponseDTO> LoginUserAsync(UserLoginDTO userLoginDTO);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> LoadUserEntityByUsernameAsync(string email);
    }
}
