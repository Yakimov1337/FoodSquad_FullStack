using System;
using System.Threading.Tasks;

namespace FoodSquad_API.Services.Interfaces
{
    public interface ITokenService
    {
        Task<bool> IsRefreshTokenValidAsync(string email, string refreshToken);
        Task SaveTokensAsync(string email, string accessToken, string refreshToken);
        Task InvalidateTokensAsync(string accessToken, string refreshToken);
        //not used for now (maybe use later to compare the exp claim of the token with the database expiration date)
        Task<bool> IsAccessTokenValidAsync(string accessToken);
    }
}
