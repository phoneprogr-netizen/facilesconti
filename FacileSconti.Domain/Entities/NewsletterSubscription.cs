using FacileSconti.Domain.Common;

namespace FacileSconti.Domain.Entities;

public class NewsletterSubscription : AuditableEntity
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool IsConfirmed { get; set; }
    public DateTime? ConfirmedAt { get; set; }
}
