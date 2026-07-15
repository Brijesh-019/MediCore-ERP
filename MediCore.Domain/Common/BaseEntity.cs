namespace MediCore.Domain.Common;

public abstract class BaseEntity
{
    public long Id { get; set; }

    public byte Status { get; set; } = 1;

    public long? CreatedBy { get; set; }

    public string? CreatedIp { get; set; }
    
    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public string? ModifiedIp { get; set; }

    public DateTime? ModifiedDate { get; set; }
}