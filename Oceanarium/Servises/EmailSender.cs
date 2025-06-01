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
        private readonly string _baseUrl;

        private string? GetLinkForMessageType(EmailMessageType type, string? code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return null;

            var paths = new Dictionary<EmailMessageType, string>
            {
                { EmailMessageType.OrderConfirmation, $"/OrderCancel?code={code}" },
            };

            return paths.TryGetValue(type, out var path) ? $"{_baseUrl}{path}" : null;
        }

        public EmailSender(IOptions<MailSettings> mailSettings, IConfiguration configuration)
        {
            _mailSettings = mailSettings.Value;
            _baseUrl = configuration["AppSettings:BaseUrl"] ?? "https://localhost:7102";
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage, EmailMessageType messageType = EmailMessageType.Generic, byte[] qrCodeImage = null, string? code = null)
        {
            var client = new SendGridClient(_mailSettings.SendGridApiKey);

            var from = new EmailAddress(_mailSettings.SenderEmail, _mailSettings.SenderName);
            var to = new EmailAddress(toEmail);

            string? link = GetLinkForMessageType(messageType, code);
            if (!string.IsNullOrEmpty(link))
            {
                htmlMessage += $"<br/><a href='{link}'>{link}</a>";
            }

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlContent: htmlMessage);

            if (qrCodeImage != null)
            {
                string base64Content = Convert.ToBase64String(qrCodeImage);

                msg.AddAttachment("qrCodeImage.png", base64Content, "image/png", "attachment", "qrCodeImage");
            }

            var response = await client.SendEmailAsync(msg);

            Console.WriteLine($"StatusCode: {response.StatusCode}");
            Console.WriteLine(await response.Body.ReadAsStringAsync());

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Body.ReadAsStringAsync();
                throw new Exception($"Error while sending email через SendGrid: {response.StatusCode}, {body}");
            }
        }
    }
}
