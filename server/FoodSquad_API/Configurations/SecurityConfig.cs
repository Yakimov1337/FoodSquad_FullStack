using FoodSquad_API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

public static class SecurityConfig
{
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtUtil = new JwtUtil(configuration);
        var tokenValidationParameters = jwtUtil.GetTokenValidationParameters();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
         .AddJwtBearer(options =>
         {
             options.TokenValidationParameters = tokenValidationParameters;
             options.MapInboundClaims = false; // Disable automatic claim mapping
             options.Events = new JwtBearerEvents
             {
                 OnTokenValidated = context =>
                 {
                     var identity = context.Principal.Identity as ClaimsIdentity;

                     if (identity != null)
                     {
                         // Map "role" claim to ClaimTypes.Role
                         var roleClaim = identity.FindFirst("role");
                         if (roleClaim != null)
                         {
                             identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                         }

                         // Map "email" claim to ClaimTypes.Email
                         var emailClaim = identity.FindFirst("email"); // Access "email" directly from the JWT
                         if (emailClaim != null)
                         {
                             identity.AddClaim(new Claim(ClaimTypes.Email, emailClaim.Value));
                         }
                     }

                     return Task.CompletedTask;
                 }
             };

         });

        return services;
    }


    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
            options.AddPolicy("ModeratorPolicy", policy => policy.RequireRole("Moderator", "Admin"));
            options.AddPolicy("UserPolicy", policy => policy.RequireRole("Normal", "Moderator", "Admin"));
        });

        // Add custom handler for access denied
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAccessDeniedHandler>();

        return services;
    }
}