using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace MortgageComparerAPI.Services.Interfaces {
    public interface IEmailService {

        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);

        Task SendEmailWithAttachmentAsync(string toEmail, string subject, string htmlMessage, string fileName, byte[] fileContent);
    }

}
