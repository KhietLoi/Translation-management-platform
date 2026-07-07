using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MySolution.Application.Common.Interfaces;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Authentication;


public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public JwtService(IOptions<JwtSettings> options)
    {
        _jwtSettings = options.Value;
    }


    /// Tạo Access Token.
    public string GenerateJwtToken(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var claims = BuildClaims(user);

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
            signingCredentials: credentials);

        return _tokenHandler.WriteToken(token);
    }


    /// Tạo Refresh Token 
    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));
    }

 
    /// Đọc Claims từ Access Token đã hết hạn.
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,

            // Không kiểm tra thời gian hết hạn
            ValidateLifetime = false,

            ValidateIssuerSigningKey = true,

            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.SecretKey))
        };

        var principal = _tokenHandler.ValidateToken(
            token,
            tokenValidationParameters,
            out var securityToken);

        if (securityToken is not JwtSecurityToken jwtToken)
        {
            throw new SecurityTokenException("Invalid JWT token.");
        }

        if (!jwtToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new SecurityTokenException("Invalid signing algorithm.");
        }

        return principal;
    }
    
    /// Tạo danh sách Claims của người dùng.
    private static List<Claim> BuildClaims(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            new(JwtRegisteredClaimNames.Email, user.Email),

            new(ClaimTypes.NameIdentifier, user.Id.ToString()),

            new(ClaimTypes.Name, user.Username),

            new(ClaimTypes.Email, user.Email)
        };

        // Roles
        claims.AddRange(
            user.UserRoles.Select(userRole =>
                new Claim(
                    ClaimTypes.Role,
                    userRole.Role.Name)));

        // Permissions
        claims.AddRange(
            user.UserRoles
                .SelectMany(userRole => userRole.Role.RolePermissions)
                .Select(rolePermission =>
                    new Claim(
                        "permission",
                        rolePermission.Permission.Code)));

        return claims;
    }
}