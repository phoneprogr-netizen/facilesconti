using FacileSconti.Domain.Common;

namespace FacileSconti.Domain.Entities;

public class CustomerBusiness : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string VatNumber { get; set; } = string.Empty;
    public string FiscalCode { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoPath { get; set; }
    public string OwnerUserId { get; set; } = string.Empty;

    public ApplicationUser OwnerUser { get; set; } = default!;
    public ICollection<CustomerContact> Contacts { get; set; } = new List<CustomerContact>();
    public ICollection<CustomerContract> Contracts { get; set; } = new List<CustomerContract>();
    public ICollection<Coupon> Coupons { get; set; } = new List<Coupon>();
}
