namespace Oceanarium.Servises.Interfaces
{
    public interface IQrCodeCreator
    {
        byte[] GenerateQrCode(string content);
    }
}
