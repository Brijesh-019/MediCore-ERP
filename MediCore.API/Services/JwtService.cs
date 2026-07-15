using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MediCore.API.Configuration;
using MediCore.Application.DTOs;
using MediCore.Application.Interfaces;
using MediCore.Shared.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MediCore.API.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;

    public JwtService(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    public TokenResult GenerateTokens(
    long userId,
    string userName,
    string sessionId)
    {
        var currentDateTime = DateTime.UtcNow;

        var accessTokenExpiry = currentDateTime.AddMinutes(
            _jwtSettings.AccessTokenMinutes
        );

        var refreshTokenExpiry = currentDateTime.AddDays(
            _jwtSettings.RefreshTokenDays
        );

        var claims = new List<Claim>
    {
        new(
            ClaimTypes.NameIdentifier,
            userId.ToString()
        ),

        new(
            ClaimTypes.Name,
            userName
        ),

        new(
            JwtRegisteredClaimNames.Sub,
            userId.ToString()
        ),

        new(
            JwtRegisteredClaimNames.Jti,
            Guid.NewGuid().ToString("N")
        ),

        new(
            "session_id",
            sessionId
        )
    };

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)
        );

        var signingCredentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256
        );

        var jwtToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: currentDateTime,
            expires: accessTokenExpiry,
            signingCredentials: signingCredentials
        );

        var accessToken =
            new JwtSecurityTokenHandler().WriteToken(jwtToken);

        var refreshToken = GenerateRefreshToken();

        return new TokenResult
        {
            AccessToken = accessToken,
            AccessTokenExpiry = accessTokenExpiry,
            RefreshToken = refreshToken,
            RefreshTokenExpiry = refreshTokenExpiry
        };
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }
}