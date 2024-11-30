using FoodSquad_API.Models.Entity;
using System;
using System.Threading.Tasks;

namespace FoodSquad_API.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        Task<Token> FindByUserAndAccessTokenAsync(Guid userId, string accessToken);
        Task<Token> FindByUserAndRefreshTokenAsync(Guid userId, string refreshToken);
        Task<bool> ExistsByAccessTokenAsync(string accessToken);
        Task SaveAsync(Token token);
        Task DeleteByUserAsync(Guid userId);
        Task DeleteByAccessTokenAsync(string accessToken);
        Task DeleteByRefreshTokenAsync(string refreshToken);
        Task<Token> FindByAccessTokenAsync(string accessToken);
    }
}
