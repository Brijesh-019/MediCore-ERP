using MediCore.Application.DTOs;
using MediCore.Application.Interfaces;
using MediCore.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MediCore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;

    public AuthController(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
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

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash
        );

        if (!isPasswordValid)
        {
            return Unauthorized(
                ApiResponse<object>.Fail(
                    "Invalid username or password.",
                    401
                )
            );
        }

        var hospitals =
            await _authRepository.GetUserHospitalsAsync(user.UserId);

        user.Hospitals = hospitals;

        if (hospitals.Count == 0)
        {
            return Unauthorized(
                ApiResponse<object>.Fail(
                    "No hospital access assigned to this user.",
                    401
                )
            );
        }

        if (hospitals.Count == 1)
        {
            user.SelectedHospitalId = hospitals[0].HospitalId;

            var branches =
                await _authRepository.GetUserBranchesAsync(
                    user.UserId,
                    hospitals[0].HospitalId
                );

            user.Branches = branches;

            if (branches.Count == 1)
            {
                user.SelectedBranchId = branches[0].BranchId;
            }

            user.RequiresHospitalSelection = false;
            user.RequiresBranchSelection = branches.Count > 1;
        }
        else
        {
            user.RequiresHospitalSelection = true;
            user.RequiresBranchSelection = false;
        }

        user.PasswordHash = string.Empty;

        return Ok(
            ApiResponse<LoginResponse>.Ok(
                user,
                "Login credentials verified successfully."
            )
        );
    }
}