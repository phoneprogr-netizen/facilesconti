namespace FacileSconti.Infrastructure.Options;

public sealed class TurboSmtpOptions
{
    public const string SectionName = "TurboSmtp";

    public string ApiUrl { get; set; } = "https://api.turbo-smtp.com/api/v2/mail/send";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? ApiKey { get; set; }
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = "FacileSconti";
}
