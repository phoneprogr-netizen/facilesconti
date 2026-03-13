using FacileSconti.Domain.Common;

namespace FacileSconti.Domain.Entities;

public class CustomerContact : AuditableEntity
{
    public int Id { get; set; }
    public int CustomerBusinessId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;

    public CustomerBusiness CustomerBusiness { get; set; } = default!;
}
