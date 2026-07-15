using System.ComponentModel.DataAnnotations;

namespace MediCore.Application.DTOs;

public class CreateHospitalRequest
{
    [Required]
    [MaxLength(20)]
    public string HospitalCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string HospitalName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? HospitalShortName { get; set; }

    [EmailAddress]
    [MaxLength(150)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Mobile { get; set; }

    [MaxLength(20)]
    public string? AlternateMobile { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(10)]
    public string? Pincode { get; set; }

    [MaxLength(500)]
    public string? Logo { get; set; }

    [MaxLength(50)]
    public string? GstNumber { get; set; }

    [MaxLength(50)]
    public string? PanNumber { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public byte Status { get; set; } = 1;
}