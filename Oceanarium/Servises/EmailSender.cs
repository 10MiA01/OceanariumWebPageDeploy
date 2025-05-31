using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Oceanarium.Servises.Interfaces;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace Oceanarium.Servises
{
    public class EmailSender : IEmailSender
    {
        private readonly MailSettings _mailSettings;

        public EmailSender(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage, byte[] qrCodeImage = null)
        {
            var client = new SendGridClient(_mailSettings.SendGridApiKey);

            var from = new EmailAddress(_mailSettings.SenderEmail, _mailSettings.SenderName);
            var to = new EmailAddress(toEmail);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlContent: htmlMessage);

            if (qrCodeImage != null)
            {
                string base64Content = Convert.ToBase64String(qrCodeImage);

                msg.AddAttachment("qrCodeImage.png", base64Content, "image/png", "attachment", "qrCodeImage");
            }

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Body.ReadAsStringAsync();
                throw new Exception($"Error while sending email через SendGrid: {response.StatusCode}, {body}");
            }
        }
    }
}
