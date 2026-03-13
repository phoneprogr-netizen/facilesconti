using FacileSconti.Domain.Common;

namespace FacileSconti.Domain.Entities;

public class HomeBoostPlan : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public decimal Price { get; set; }
    public int Priority { get; set; }

    public ICollection<CustomerBoostActivation> Activations { get; set; } = new List<CustomerBoostActivation>();
}
