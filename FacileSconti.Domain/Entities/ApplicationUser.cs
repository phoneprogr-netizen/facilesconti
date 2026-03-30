using Microsoft.AspNetCore.Identity;
using FacileSconti.Domain.Common;
using FacileSconti.Domain.Enums;

namespace FacileSconti.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserType UserType { get; set; } = UserType.EndUser;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public CustomerBusiness? CustomerBusiness { get; set; }
    public ICollection<CouponDownload> CouponDownloads { get; set; } = new List<CouponDownload>();
}
