using FoodSquad_API.Data;
using FoodSquad_API.Models.Entity;
using FoodSquad_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace FoodSquad_API.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly MyDbContext _dbContext;

        public TokenRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Token> FindByUserAndAccessTokenAsync(Guid userId, string accessToken)
        {
            return await _dbContext.Tokens
                .FirstOrDefaultAsync(t => t.UserId == userId && t.AccessToken == accessToken);
        }

        public async Task<Token> FindByUserAndRefreshTokenAsync(Guid userId, string refreshToken)
        {
            return await _dbContext.Tokens
                .FirstOrDefaultAsync(t => t.UserId == userId && t.RefreshToken == refreshToken);
        }

        public async Task<bool> ExistsByAccessTokenAsync(string accessToken)
        {
            return await _dbContext.Tokens.AnyAsync(t => t.AccessToken == accessToken);
        }

        public async Task SaveAsync(Token token)
        {
            _dbContext.Tokens.Add(token);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteByUserAsync(Guid userId)
        {
            var tokens = _dbContext.Tokens.Where(t => t.UserId == userId);
            _dbContext.Tokens.RemoveRange(tokens);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteByAccessTokenAsync(string accessToken)
        {
            var token = await _dbContext.Tokens.FirstOrDefaultAsync(t => t.AccessToken == accessToken);
            if (token != null)
            {
                _dbContext.Tokens.Remove(token);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteByRefreshTokenAsync(string refreshToken)
        {
            var token = await _dbContext.Tokens.FirstOrDefaultAsync(t => t.RefreshToken == refreshToken);
            if (token != null)
            {
                _dbContext.Tokens.Remove(token);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<Token?> FindByAccessTokenAsync(string accessToken)
        {
            return await _dbContext.Tokens.FirstOrDefaultAsync(t => t.AccessToken == accessToken);
        }

    }
}
