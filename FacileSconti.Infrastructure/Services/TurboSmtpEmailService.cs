using System.Net.Http.Json;
using System.Net;
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
        var html = BuildEmailTemplate(
            title: $"Ciao {safeName}, benvenuto su FacileSconti!",
            subtitle: "Il tuo account è stato creato con successo.",
            bodyHtml: """
                      <p style="margin: 0 0 16px 0;">Da ora puoi sfogliare i coupon, scaricarli e usarli nei negozi aderenti.</p>
                      <p style="margin: 0;">Qui sotto trovi un accesso di esempio (fittizio):</p>
                      <div style="margin-top: 12px; padding: 12px; border: 1px dashed #cbd5e1; border-radius: 8px; background-color: #f8fafc;">
                        <p style="margin: 0 0 8px 0;"><strong>Login demo:</strong> demo.utente@facilesconti.local</p>
                        <p style="margin: 0;"><strong>Password demo:</strong> facile123!</p>
                      </div>
                      """,
            ctaText: "Accedi a FacileSconti",
            ctaUrl: "https://app.facilesconti.local/login");

        return SendAsync(toEmail, safeName, subject, html, cancellationToken);
    }

    public Task SendCouponDownloadedEmailAsync(string toEmail, string recipientName, string couponTitle, string couponCode, CancellationToken cancellationToken = default)
    {
        var safeName = string.IsNullOrWhiteSpace(recipientName) ? "Cliente" : recipientName;
        var subject = "Hai scaricato un coupon";
        var html = BuildEmailTemplate(
            title: $"Ciao {safeName},",
            subtitle: "Hai scaricato con successo un coupon.",
            bodyHtml: $"""
                       <p style="margin: 0 0 16px 0;">Hai scaricato il coupon <strong>{HtmlEncode(couponTitle)}</strong>.</p>
                       <p style="margin: 0 0 16px 0;">Codice coupon: <strong>{HtmlEncode(couponCode)}</strong></p>
                       <p style="margin: 0;">Mostra questo codice in cassa per usufruire dello sconto.</p>
                       """,
            ctaText: "Vai ai miei coupon",
            ctaUrl: "https://app.facilesconti.local/coupon");

        return SendAsync(toEmail, safeName, subject, html, cancellationToken);
    }

    public Task SendCouponPublishedEmailAsync(string toEmail, string customerName, string couponTitle, DateOnly validTo, CancellationToken cancellationToken = default)
    {
        var safeName = string.IsNullOrWhiteSpace(customerName) ? "Cliente" : customerName;
        var subject = "Il tuo coupon è ora online";
        var html = BuildEmailTemplate(
            title: $"Ciao {safeName},",
            subtitle: "Il tuo coupon è ora online.",
            bodyHtml: $"""
                       <p style="margin: 0 0 16px 0;">Il coupon <strong>{HtmlEncode(couponTitle)}</strong> è stato pubblicato ed è ora visibile agli utenti.</p>
                       <p style="margin: 0;">Scadenza coupon: <strong>{validTo:dd/MM/yyyy}</strong></p>
                       """,
            ctaText: "Apri pannello coupon",
            ctaUrl: "https://app.facilesconti.local/admin/coupon");

        return SendAsync(toEmail, safeName, subject, html, cancellationToken);
    }

    private static string BuildEmailTemplate(string title, string subtitle, string bodyHtml, string ctaText, string ctaUrl)
    {
        return $"""
                <!doctype html>
                <html lang="it">
                <head>
                  <meta charset="utf-8">
                  <meta name="viewport" content="width=device-width, initial-scale=1.0">
                  <title>{HtmlEncode(title)}</title>
                </head>
                <body style="margin:0; padding:0; background-color:#eef2ff; font-family:Arial, Helvetica, sans-serif; color:#0f172a;">
                  <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="padding: 24px 12px;">
                    <tr>
                      <td align="center">
                        <table role="presentation" width="600" cellspacing="0" cellpadding="0" style="width:100%; max-width:600px; border-collapse:separate; background-color:#ffffff; border-radius:14px; overflow:hidden; box-shadow: 0 8px 24px rgba(15,23,42,0.08);">
                          <tr>
                            <td style="padding: 24px; background: linear-gradient(135deg, #4f46e5, #7c3aed); color:#ffffff;">
                              <p style="margin:0; font-size:14px; letter-spacing:0.3px;">FacileSconti</p>
                              <h1 style="margin:8px 0 0 0; font-size:24px; line-height:30px;">{HtmlEncode(title)}</h1>
                              <p style="margin:8px 0 0 0; font-size:15px; opacity:0.95;">{HtmlEncode(subtitle)}</p>
                            </td>
                          </tr>
                          <tr>
                            <td style="padding: 24px;">
                              {bodyHtml}
                              <div style="margin-top: 24px;">
                                <a href="{HtmlEncode(ctaUrl)}" style="display:inline-block; padding: 12px 18px; background-color:#4f46e5; color:#ffffff; text-decoration:none; border-radius:8px; font-weight:600;">
                                  {HtmlEncode(ctaText)}
                                </a>
                              </div>
                            </td>
                          </tr>
                          <tr>
                            <td style="padding:16px 24px; background-color:#f8fafc; border-top:1px solid #e2e8f0;">
                              <p style="margin:0 0 6px 0; font-size:12px; color:#475569;">Hai ricevuto questa email da <strong>FacileSconti</strong>.</p>
                              <p style="margin:0; font-size:12px; color:#64748b;">Messaggio automatico, non rispondere direttamente a questa email.</p>
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                  </table>
                </body>
                </html>
                """;
    }

    private static string HtmlEncode(string value) => WebUtility.HtmlEncode(value);

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
