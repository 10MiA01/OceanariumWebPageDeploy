namespace Oceanarium.Servises.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage,
            EmailMessageType messageType = EmailMessageType.Generic,
            byte[] qrCodeImage = null, string? code = null);
    }
}
