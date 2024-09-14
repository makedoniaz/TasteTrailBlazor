using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace TasteTrailBlazor.Providers;

public static class JwtTokenHelper
{
    public static List<Claim> ValidateDecodeToken(string token, IConfiguration configuration)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = configuration["Jwt:Key"];
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        try
        {
            tokenHandler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey( System.Text.Encoding.UTF8.GetBytes(key!)),
                    ClockSkew = TimeSpan.FromMinutes(15)
                },
                out var validatedToken
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return new List<Claim>();
        }
        

        var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
        return securityToken?.Claims.ToList()!;
    }
}
