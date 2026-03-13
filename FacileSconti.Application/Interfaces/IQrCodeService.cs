namespace FacileSconti.Application.Interfaces;

public interface IQrCodeService
{
    string GenerateSvgDataUri(string payload);
}
