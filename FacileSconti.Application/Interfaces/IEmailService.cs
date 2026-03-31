namespace FacileSconti.Application.Interfaces;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string toEmail, string recipientName, CancellationToken cancellationToken = default);
    Task SendCouponDownloadedEmailAsync(string toEmail, string recipientName, string couponTitle, string couponCode, CancellationToken cancellationToken = default);
    Task SendCouponPublishedEmailAsync(string toEmail, string customerName, string couponTitle, DateOnly validTo, CancellationToken cancellationToken = default);
}
