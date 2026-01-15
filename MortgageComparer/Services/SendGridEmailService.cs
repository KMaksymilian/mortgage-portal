using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using MortgageComparer.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace MortgageComparer.Services {
    public class SendGridEmailService : IEmailService {
        private readonly ISendGridClient _client; 
        private readonly IConfiguration _configuration;

        public SendGridEmailService(ISendGridClient client, IConfiguration configuration) {
            _client = client;
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage) {
            var msg = CreateMessage(toEmail, subject, htmlMessage);
            var response = await _client.SendEmailAsync(msg);
            ValidateResponse(response);
        }

        public async Task SendEmailWithAttachmentAsync(string toEmail, string subject, string htmlMessage, string fileName, byte[] fileContent) {
            var msg = CreateMessage(toEmail, subject, htmlMessage);

            if (fileContent != null && fileContent.Length > 0) {
                var fileBase64 = Convert.ToBase64String(fileContent);
                msg.AddAttachment(fileName, fileBase64);
            }

            var response = await _client.SendEmailAsync(msg);
            ValidateResponse(response);
        }

        private SendGridMessage CreateMessage(string toEmail, string subject, string htmlContent) {
            var fromEmail = _configuration["SendGrid:FromEmail"];
            var fromName = _configuration["SendGrid:FromName"];

            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(toEmail);

            return MailHelper.CreateSingleEmail(from, to, subject, "*", htmlContent);
        }

        private void ValidateResponse(Response response) {
            if (!response.IsSuccessStatusCode) {
                throw new Exception($"SendGrid Error: {response.StatusCode}");
            }
        }
    }
}