using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TaskTrack.Service.DTOs;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.API.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public LoginResponseDto CreateLoginResponse(AccountDto account)
    {
        var secret = GetJwtSecret(_configuration);
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var expiryHours = _configuration.GetValue("Jwt:ExpiryHours", 24);
        var expiresAt = DateTime.UtcNow.AddHours(expiryHours);
        var roleName = account.Role == 1 ? "Admin" : "Staff";

        var claims = new List<Claim>
        {
            new("AccountID", account.AccountId.ToString(CultureInfo.InvariantCulture)),
            new("Email", account.Email),
            new("Role", roleName),
            new("RoleId", account.Role.ToString(CultureInfo.InvariantCulture))
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new LoginResponseDto(new JwtSecurityTokenHandler().WriteToken(token), expiresAt, account);
    }

    public static string GetJwtSecret(IConfiguration configuration)
    {
        var secret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? configuration["Jwt:Secret"];
        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException("JWT secret is missing. Configure JWT_SECRET environment variable.");
        }

        if (Encoding.UTF8.GetByteCount(secret) < 32)
        {
            throw new InvalidOperationException("JWT secret must be at least 32 bytes.");
        }

        return secret;
    }
}
