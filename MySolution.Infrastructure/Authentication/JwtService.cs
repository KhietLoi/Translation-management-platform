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
    public JwtService(IOptions<JwtSettings> options)
    {
        _jwtSettings = options.Value;
    }
    
    public string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
         
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
           
        };

        // Roles
        claims.AddRange(
            user.UserRoles.Select(x =>
                new Claim(ClaimTypes.Role, x.Role.Name)));

        // Permissions
        claims.AddRange(
            user.UserRoles
                .SelectMany(x => x.Role.RolePermissions)
                .Select(x =>
                    new Claim("permission", x.Permission.Code)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);

    }
    
    
    //Cap lai token moi
    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false, // Cho phép đọc token đã hết hạn
            ValidateIssuerSigningKey = true,

            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.SecretKey))
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(
            token,
            tokenValidationParameters,
            out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token.");
        }

        return principal;

    }
}