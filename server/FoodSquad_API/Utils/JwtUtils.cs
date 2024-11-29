using FoodSquad_API.Models.Entity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FoodSquad_API.Utils
{
    public class JwtUtil
    {
        private readonly string _secret;
        public readonly long AccessTokenExpiration;
        public readonly long RefreshTokenExpiration;

        public JwtUtil(IConfiguration configuration)
        {
            var jwtConfig = configuration.GetSection("Jwt");
            _secret = jwtConfig["Key"] ?? throw new InvalidOperationException("JWT Secret is missing.");
            AccessTokenExpiration = long.Parse(jwtConfig["AccessTokenExpirationMinutes"]) * 60 * 1000; 
            RefreshTokenExpiration = long.Parse(jwtConfig["RefreshTokenExpirationDays"]) * 24 * 60 * 60 * 1000; 
        }

        public string GenerateToken(IDictionary<string, object> claims, string subject, long expiration)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("sub", subject) }),
                Expires = DateTime.UtcNow.AddMilliseconds(expiration),
                Claims = claims,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public IDictionary<string, object> ExtractClaims(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
                throw new SecurityTokenException("Invalid token");

            var claims = new Dictionary<string, object>();
            foreach (var claim in jwtToken.Claims)
            {
                claims.Add(claim.Type, claim.Value);
            }

            return claims;
        }

        public bool ValidateToken(string token, User userDetails)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            var handler = new JwtSecurityTokenHandler();

            try
            {
                var principal = handler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if (validatedToken is JwtSecurityToken jwtToken && jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    var email = principal.FindFirst("sub")?.Value;
                    return userDetails.Email == email;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

    }
}
