using MediCore.Application.DTOs;

namespace MediCore.Application.Interfaces;

public interface IAuthRepository
{
    Task<LoginResponse?> GetLoginUserAsync(
        string userName
    );

    Task<List<HospitalAccessDto>> GetUserHospitalsAsync(
        long userId
    );

    Task<List<BranchAccessDto>> GetUserBranchesAsync(
        long userId,
        long hospitalId
    );

    Task<(bool Success, string Message, long Id)>
        CreateUserSessionAsync(
            CreateUserSessionRequest request
        );

    Task<RefreshSessionDto?> GetRefreshSessionAsync(
        string refreshTokenHash
    );

    Task<(bool Success, string Message)>
        RotateRefreshTokenAsync(
            string sessionId,
            string oldRefreshTokenHash,
            string newRefreshTokenHash,
            DateTime newRefreshExpiryDate,
            string? modifyIp
        );
}