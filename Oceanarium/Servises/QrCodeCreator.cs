using Oceanarium.Servises.Interfaces;
using QRCoder;

namespace Oceanarium.Servises
{
    public class QrCodeCreator : IQrCodeCreator
    {
        public byte[] GenerateQrCode(string content)
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);
            return qrCode.GetGraphic(20);
        }
    }
}
