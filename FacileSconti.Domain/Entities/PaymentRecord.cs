using FacileSconti.Domain.Common;
using FacileSconti.Domain.Enums;

namespace FacileSconti.Domain.Entities;

public class PaymentRecord : AuditableEntity
{
    public long Id { get; set; }
    public int CustomerContractId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? ExternalReference { get; set; }

    public CustomerContract CustomerContract { get; set; } = default!;
}
