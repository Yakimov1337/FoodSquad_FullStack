using FoodSquad_API.Models.Enums;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

    public string GenerateToken(IDictionary<string, object> claims, string email, UserRole role, long expiration)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Create a dictionary to ensure no duplicate keys in claims
        var finalClaims = new Dictionary<string, object>
    {
        { "email", email },  // Add email as a single claim
        { "role", role.ToString() },  // Add role as a single claim
        { "id", claims.ContainsKey("id") ? claims["id"] : null }  // Add id only if it exists
    };

        // Merge additional claims without overwriting existing ones
        foreach (var claim in claims)
        {
            if (!finalClaims.ContainsKey(claim.Key))
            {
                finalClaims.Add(claim.Key, claim.Value);
            }
        }


        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(finalClaims.Select(c => new Claim(c.Key, c.Value?.ToString() ?? string.Empty))),
            Expires = DateTime.UtcNow.AddMilliseconds(expiration),
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

        // Directly parse the payload
        return jwtToken.Payload.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public TokenValidationParameters GetTokenValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }
}
