namespace Oceanarium.Servises.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage, byte[] qrCodeImage = null);
    }
}
