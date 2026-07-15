using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MediCore.Application.DTOs;
using MediCore.Application.Interfaces;
using MediCore.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediCore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtService _jwtService;

    public AuthController(
        IAuthRepository authRepository,
        IJwtService jwtService)
    {
        _authRepository = authRepository;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
        {
            return BadRequest(
                ApiResponse<object>.Fail(
                    "Username is required.",
                    400
                )
            );
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(
                ApiResponse<object>.Fail(
                    "Password is required.",
                    400
                )
            );
        }

        var user = await _authRepository.GetLoginUserAsync(
            request.UserName.Trim()
        );

        if (user is null)
        {
            return Unauthorized(
                ApiResponse<object>.Fail(
                    "Invalid username or password.",
                    401
                )
            );
        }

        var passwordIsValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash
        );

        if (!passwordIsValid)
        {
            return Unauthorized(
                ApiResponse<object>.Fail(
                    "Invalid username or password.",
                    401
                )
            );
        }

        var hospitals =
            await _authRepository.GetUserHospitalsAsync(
                user.UserId
            );

        if (hospitals.Count == 0)
        {
            return Unauthorized(
                ApiResponse<object>.Fail(
                    "No hospital access assigned to this user.",
                    401
                )
            );
        }

        user.Hospitals = hospitals;

        long? selectedHospitalId = null;
        long? selectedBranchId = null;

        if (hospitals.Count == 1)
        {
            selectedHospitalId = hospitals[0].HospitalId;

            user.SelectedHospitalId = selectedHospitalId;
            user.RequiresHospitalSelection = false;

            var branches =
                await _authRepository.GetUserBranchesAsync(
                    user.UserId,
                    selectedHospitalId.Value
                );

            user.Branches = branches;

            if (branches.Count == 1)
            {
                selectedBranchId = branches[0].BranchId;

                user.SelectedBranchId = selectedBranchId;
                user.RequiresBranchSelection = false;
            }
            else
            {
                user.RequiresBranchSelection =
                    branches.Count > 1;
            }
        }
        else
        {
            user.RequiresHospitalSelection = true;
            user.RequiresBranchSelection = false;
        }

        var sessionId = Guid.NewGuid().ToString("N");

        var tokenResult = _jwtService.GenerateTokens(
    user.UserId,
    user.UserName,
    sessionId
);

        var refreshTokenHash = HashToken(
            tokenResult.RefreshToken
        );

        var sessionRequest =
            new CreateUserSessionRequest
            {
                SessionId = sessionId,
                UserId = user.UserId,
                ActiveHospitalId = selectedHospitalId,
                ActiveBranchId = selectedBranchId,
                RefreshTokenHash = refreshTokenHash,
                TokenVersion = 1,
                RefreshExpiryDate =
                    tokenResult.RefreshTokenExpiry,
                IpAddress =
                    HttpContext.Connection
                        .RemoteIpAddress?
                        .ToString(),
                UserAgent =
                    Request.Headers.UserAgent.ToString()
            };

        var sessionResult =
            await _authRepository.CreateUserSessionAsync(
                sessionRequest
            );

        if (!sessionResult.Success)
        {
            return StatusCode(
                500,
                ApiResponse<object>.Fail(
                    sessionResult.Message,
                    500
                )
            );
        }

        SaveAuthenticationCookies(tokenResult);

        user.PasswordHash = string.Empty;

        var loginResponse = new LoginTokenResponse
        {
            User = user,

            // Swagger testing માટે temporary.
            AccessToken = tokenResult.AccessToken,

            AccessTokenExpiry =
                tokenResult.AccessTokenExpiry,

            RefreshTokenExpiry =
                tokenResult.RefreshTokenExpiry
        };

        return Ok(
            ApiResponse<LoginTokenResponse>.Ok(
                loginResponse,
                "Login successful."
            )
        );
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        var userName = User.Identity?.Name;

        var sessionId = User.FindFirstValue(
            "session_id"
        );

        return Ok(
            ApiResponse<object>.Ok(
                new
                {
                    userId,
                    userName,
                    sessionId
                },
                "Authorized user details loaded successfully."
            )
        );
    }

    private void SaveAuthenticationCookies(
        MediCore.Shared.Models.TokenResult tokenResult)
    {
        Response.Cookies.Append(
            "access_token",
            tokenResult.AccessToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires =
                    new DateTimeOffset(
                        tokenResult.AccessTokenExpiry
                    ),
                Path = "/",
                IsEssential = true
            }
        );

        Response.Cookies.Append(
            "refresh_token",
            tokenResult.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires =
                    new DateTimeOffset(
                        tokenResult.RefreshTokenExpiry
                    ),
                Path = "/api/Auth",
                IsEssential = true
            }
        );
    }

    private static string HashToken(string token)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(token);

        var hashBytes = SHA256.HashData(tokenBytes);

        return Convert.ToHexString(hashBytes);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        if (!Request.Cookies.TryGetValue(
                "refresh_token",
                out var refreshToken) ||
            string.IsNullOrWhiteSpace(refreshToken))
        {
            DeleteAuthenticationCookies();

            return Unauthorized(
                ApiResponse<object>.Fail(
                    "Refresh token was not found.",
                    401
                )
            );
        }

        var oldRefreshTokenHash = HashToken(
            refreshToken
        );

        var session =
            await _authRepository.GetRefreshSessionAsync(
                oldRefreshTokenHash
            );

        if (session is null ||
            session.IsRevoked ||
            session.RefreshExpiryDate <= DateTime.Now)
        {
            DeleteAuthenticationCookies();

            return Unauthorized(
                ApiResponse<object>.Fail(
                    "Refresh token is invalid or expired.",
                    401
                )
            );
        }

        var tokenResult = _jwtService.GenerateTokens(
            session.UserId,
            session.UserName,
            session.SessionId
        );

        var newRefreshTokenHash = HashToken(
            tokenResult.RefreshToken
        );

        var updateResult =
            await _authRepository.RotateRefreshTokenAsync(
                session.SessionId,
                oldRefreshTokenHash,
                newRefreshTokenHash,
                tokenResult.RefreshTokenExpiry,
                HttpContext.Connection
                    .RemoteIpAddress?
                    .ToString()
            );

        if (!updateResult.Success)
        {
            DeleteAuthenticationCookies();

            return Unauthorized(
                ApiResponse<object>.Fail(
                    updateResult.Message,
                    401
                )
            );
        }

        SaveAuthenticationCookies(tokenResult);

        var response = new RefreshTokenResponse
        {
            AccessToken = tokenResult.AccessToken,
            AccessTokenExpiry =
                tokenResult.AccessTokenExpiry,
            RefreshTokenExpiry =
                tokenResult.RefreshTokenExpiry
        };

        return Ok(
            ApiResponse<RefreshTokenResponse>.Ok(
                response,
                "Access token refreshed successfully."
            )
        );
    }

    private void DeleteAuthenticationCookies()
    {
        Response.Cookies.Delete(
            "access_token",
            new CookieOptions
            {
                Path = "/"
            }
        );

        Response.Cookies.Delete(
            "refresh_token",
            new CookieOptions
            {
                Path = "/api/Auth"
            }
        );
    }
}