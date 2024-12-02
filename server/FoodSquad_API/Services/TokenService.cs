using FoodSquad_API.Models.Entity;
using FoodSquad_API.Repositories.Interfaces;
using FoodSquad_API.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace FoodSquad_API.Services
{
    public class TokenService : ITokenService
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;

        public TokenService(ITokenRepository tokenRepository, IUserRepository userRepository)
        {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> IsRefreshTokenValidAsync(string email, string refreshToken)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) throw new Exception("User not found");

            var token = await _tokenRepository.FindByUserAndRefreshTokenAsync(user.Id, refreshToken);
            return token != null && token.RefreshTokenExpiryDate > DateTime.UtcNow;
        }

        public async Task SaveTokensAsync(string email, string accessToken, string refreshToken)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) throw new Exception("User not found");

            // Invalidate all other sessions for this user
            await _tokenRepository.DeleteByUserAsync(user.Id);

            var token = new Token
            {
                UserId = user.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiryDate = DateTime.UtcNow.AddMinutes(30),
                RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(7),
                User = user
            };

            await _tokenRepository.SaveAsync(token);
        }

        public async Task InvalidateTokensAsync(string accessToken, string refreshToken)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                await _tokenRepository.DeleteByAccessTokenAsync(accessToken);
            }

            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _tokenRepository.DeleteByRefreshTokenAsync(refreshToken);
            }
        }


        public async Task<bool> IsAccessTokenValidAsync(string accessToken)
        {
            //Check if token is expired
            var token = await _tokenRepository.FindByAccessTokenAsync(accessToken);
            return token != null && token.AccessTokenExpiryDate > DateTime.UtcNow;
        }

    }
}
