using Microsoft.Extensions.Configuration;
using Moq;
using MortgageComparer.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Xunit;

namespace MortgageComparer.Tests.Services {
    public class SendGridEmailServiceTests {
        private readonly Mock<ISendGridClient> _sendGridClientMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly SendGridEmailService _service;

        public SendGridEmailServiceTests() {

            _sendGridClientMock = new Mock<ISendGridClient>();
            _configurationMock = new Mock<IConfiguration>();

    
            _configurationMock.Setup(c => c["SendGrid:FromEmail"]).Returns("admin@loanhub.com");
            _configurationMock.Setup(c => c["SendGrid:FromName"]).Returns("Loan Hub Admin");

   
            _service = new SendGridEmailService(_sendGridClientMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task SendEmail_ShouldCallSendGridClient_WhenDataIsCorrect_Async() {
            // Arrange
            var toEmail = "user@example.com";
            var subject = "Test Subject";
            var message = "<h1>Hello</h1>";

     
            SendGridMessage capturedMessage = null;

            
            _sendGridClientMock
                .Setup(client => client.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
                .Callback<SendGridMessage, CancellationToken>((msg, token) => capturedMessage = msg)
                .ReturnsAsync(new Response(HttpStatusCode.Accepted, null, null));

            // Act
            await _service.SendEmailAsync(toEmail, subject, message);

            // Assert
  
            _sendGridClientMock.Verify(client =>
                client.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()),
                Times.Once
            );

            Assert.NotNull(capturedMessage);
            Assert.Equal(subject, capturedMessage.Personalizations[0].Subject);


            Assert.NotNull(capturedMessage.Personalizations);
            Assert.NotEmpty(capturedMessage.Personalizations);
            Assert.Equal(toEmail, capturedMessage.Personalizations[0].Tos[0].Email); 
        }

        [Fact]
        public async Task SendEmailWithAttachment_ShouldAddAttachment_WhenFileIsProvidedAsync() {
            // Arrange
            var fileName = "umowa.pdf";
            var fileContent = new byte[] { 1, 2, 3 }; 
            var successResponse = new Response(HttpStatusCode.Accepted, null, null);

            _sendGridClientMock
                .Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(successResponse);

            // Act
            await _service.SendEmailWithAttachmentAsync("user@example.com", "Sub", "Msg", fileName, fileContent);

            // Assert
            _sendGridClientMock.Verify(client =>
                client.SendEmailAsync(
                    It.Is<SendGridMessage>(msg =>
                        msg.Attachments != null &&
                        msg.Attachments.Count == 1 &&
                        msg.Attachments[0].Filename == fileName
                    ),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task SendEmail_ShouldThrowException_WhenSendGridFailsAsync() {
            // Arrange
            var errorResponse = new Response(HttpStatusCode.Unauthorized, null, null);

            _sendGridClientMock
                .Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(errorResponse);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _service.SendEmailAsync("test@test.com", "Temat", "Tresc")
            );
        }
    }
}