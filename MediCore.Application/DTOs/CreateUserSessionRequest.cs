namespace MediCore.Application.DTOs;

public class CreateUserSessionRequest
{
    public string SessionId { get; set; } = string.Empty;

    public long UserId { get; set; }

    public long? ActiveHospitalId { get; set; }

    public long? ActiveBranchId { get; set; }

    public string RefreshTokenHash { get; set; } = string.Empty;

    public int TokenVersion { get; set; } = 1;

    public DateTime RefreshExpiryDate { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }
}