using FacileSconti.Domain.Common;

namespace FacileSconti.Domain.Entities;

public class PublicPage : AuditableEntity
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ContentHtml { get; set; } = string.Empty;
    public bool ShowInFooter { get; set; }
}
