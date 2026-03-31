namespace FacileSconti.Web.Options;

public sealed class PayPalOptions
{
    public const string SectionName = "PayPal";

    public bool Enabled { get; set; }
    public string Environment { get; set; } = "Sandbox";
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = "EUR";

    public bool IsSandbox =>
        string.Equals(Environment, "Sandbox", StringComparison.OrdinalIgnoreCase);
}
