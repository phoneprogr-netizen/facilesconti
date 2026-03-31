using System.Net.Http.Json;
using System.Text.Json;
using FacileSconti.Application.Interfaces;
using FacileSconti.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FacileSconti.Infrastructure.Services;

public sealed class TurboSmtpEmailService : IEmailService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<TurboSmtpOptions> _options;
    private readonly ILogger<TurboSmtpEmailService> _logger;

    public TurboSmtpEmailService(
        IHttpClientFactory httpClientFactory,
        IOptions<TurboSmtpOptions> options,
        ILogger<TurboSmtpEmailService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
        _logger = logger;
    }

    public Task SendWelcomeEmailAsync(string toEmail, string recipientName, CancellationToken cancellationToken = default)
    {
        var safeName = string.IsNullOrWhiteSpace(recipientName) ? "Cliente" : recipientName;
        var subject = "Benvenuto su FacileSconti";
        var html = $"""
                    <h2>Ciao {safeName}, benvenuto su FacileSconti!</h2>
                    <p>Il tuo account è stato creato con successo.</p>
                    <p>Da ora puoi sfogliare i coupon, scaricarli e usarli nei negozi aderenti.</p>
                    """;

        return SendAsync(toEmail, safeName, subject, html, cancellationToken);
    }

    public Task SendCouponDownloadedEmailAsync(string toEmail, string recipientName, string couponTitle, string couponCode, CancellationToken cancellationToken = default)
    {
        var safeName = string.IsNullOrWhiteSpace(recipientName) ? "Cliente" : recipientName;
        var subject = "Hai scaricato un coupon";
        var html = $"""
                    <h2>Ciao {safeName},</h2>
                    <p>Hai scaricato con successo il coupon <strong>{couponTitle}</strong>.</p>
                    <p>Codice coupon: <strong>{couponCode}</strong></p>
                    <p>Mostra questo codice in cassa per usufruire dello sconto.</p>
                    """;

        return SendAsync(toEmail, safeName, subject, html, cancellationToken);
    }

    public Task SendCouponPublishedEmailAsync(string toEmail, string customerName, string couponTitle, DateOnly validTo, CancellationToken cancellationToken = default)
    {
        var safeName = string.IsNullOrWhiteSpace(customerName) ? "Cliente" : customerName;
        var subject = "Il tuo coupon è ora online";
        var html = $"""
                    <h2>Ciao {safeName},</h2>
                    <p>Il coupon <strong>{couponTitle}</strong> è stato pubblicato ed è ora visibile agli utenti.</p>
                    <p>Scadenza coupon: <strong>{validTo:dd/MM/yyyy}</strong></p>
                    """;

        return SendAsync(toEmail, safeName, subject, html, cancellationToken);
    }

    private async Task SendAsync(string toEmail, string toName, string subject, string htmlBody, CancellationToken cancellationToken)
    {
        var config = _options.Value;
        if (string.IsNullOrWhiteSpace(config.Username) || string.IsNullOrWhiteSpace(config.Password) || string.IsNullOrWhiteSpace(config.SenderEmail))
        {
            _logger.LogWarning("Invio email saltato: configurazione TurboSMTP incompleta.");
            return;
        }

        var payload = new
        {
            authuser = config.Username,
            authpass = config.Password,
            apikey = config.ApiKey,
            mail_from = config.SenderEmail,
            mail_from_name = config.SenderName,
            mail_to = new[]
            {
                new
                {
                    email = toEmail,
                    name = toName
                }
            },
            subject,
            html_content = htmlBody
        };

        try
        {
            using var client = _httpClientFactory.CreateClient(nameof(TurboSmtpEmailService));
            using var response = await client.PostAsJsonAsync(config.ApiUrl, payload, JsonOptions, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("TurboSMTP ha risposto con stato {StatusCode}. Body: {Body}", response.StatusCode, body);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante invio email con TurboSMTP a {Email}", toEmail);
        }
    }
}
