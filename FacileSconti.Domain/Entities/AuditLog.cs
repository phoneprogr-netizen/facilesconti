namespace FacileSconti.Domain.Entities;

public class AuditLog
{
    public long Id { get; set; }
    public string ActorUserId { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string? DataBefore { get; set; }
    public string? DataAfter { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
