namespace MediCore.Application.DTOs;

public class RefreshSessionDto
{
    public string SessionId { get; set; } = string.Empty;

    public long UserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public long? ActiveHospitalId { get; set; }

    public long? ActiveBranchId { get; set; }

    public int TokenVersion { get; set; }

    public DateTime RefreshExpiryDate { get; set; }

    public bool IsRevoked { get; set; }
}