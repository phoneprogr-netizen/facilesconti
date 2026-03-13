using FacileSconti.Application.Interfaces;

namespace FacileSconti.Infrastructure.Services;

public class QrCodeService : IQrCodeService
{
    public string GenerateSvgDataUri(string payload)
    {
        var escaped = System.Net.WebUtility.HtmlEncode(payload);
        var fakeSvg = $"<svg xmlns='http://www.w3.org/2000/svg' width='160' height='160'><rect width='160' height='160' fill='white'/><text x='10' y='80' font-size='10'>QR:{escaped}</text></svg>";
        return "data:image/svg+xml;base64," + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(fakeSvg));
    }
}
