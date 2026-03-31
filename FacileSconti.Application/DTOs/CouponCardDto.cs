namespace FacileSconti.Application.DTOs;

public class CouponCardDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string? BusinessLogoPath { get; set; }
    public string? BusinessCity { get; set; }
    public string? ImagePath { get; set; }
    public IReadOnlyList<string> ImagePaths { get; set; } = [];
    public DateOnly ValidTo { get; set; }
    public int DownloadsCount { get; set; }
    public bool IsBoostedInHome { get; set; }
}
