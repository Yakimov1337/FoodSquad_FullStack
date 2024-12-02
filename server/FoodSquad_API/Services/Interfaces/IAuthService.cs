using FoodSquad_API.Models.DTO.User;
using FoodSquad_API.Models.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FoodSquad_API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponseDTO> RegisterUserAsync(UserRegistrationDTO userRegistrationDTO);
        Task<UserResponseDTO> LoginUserAsync(UserLoginDTO userLoginDTO);
        Task<UserResponseDTO> GetCurrentUserAsync(ClaimsPrincipal userPrincipal);
        Task<User> LoadUserEntityByUsernameAsync(string email);
    }
}
