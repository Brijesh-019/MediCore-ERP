using MediCore.Domain.Common;

namespace MediCore.Domain.Entities;

public class Hospital : BaseEntity
{
    public string HospitalCode { get; set; } = string.Empty;

    public string HospitalName { get; set; } = string.Empty;

    public string? HospitalShortName { get; set; }

    public string? Email { get; set; }

    public string? Mobile { get; set; }

    public string? AlternateMobile { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public string? Pincode { get; set; }

    public string? Logo { get; set; }

    public string? GstNumber { get; set; }

    public string? PanNumber { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? ExpiryDate { get; set; }
}