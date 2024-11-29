using System.Threading.Tasks;

namespace FoodSquad_API.Services.Interfaces
{
    public interface ITokenService
    {
        Task<bool> IsRefreshTokenValidAsync(string email, string refreshToken);
        Task SaveTokensAsync(string email, string accessToken, string refreshToken);
        Task InvalidateTokensAsync(string accessToken, string refreshToken);
    }
}
