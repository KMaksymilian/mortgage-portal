using MortgageComparer.Services;
using System;
using System.Globalization;
using Xunit;

namespace MortgageComparer.Tests.Services {
    public class MockEmailTemplateServiceTests {
        private readonly MockEmailTemplateService _service;

        public MockEmailTemplateServiceTests() {
            _service = new MockEmailTemplateService();
        }

        [Fact]
        public void GetSubmissionConfirmation_ShouldContainCorrectDataAndCurrency() {
            // Arrange
            var name = "Jan Kowalski";
            var bank = "Bank PKO";
            var amount = 1234.56m;
            var expectedCurrency = amount.ToString("C", new CultureInfo("pl-PL")); // np. 1 234,56 zł

            // Act
            var result = _service.GetSubmissionConfirmation(name, bank, amount);

            // Assert
            Assert.Contains("Wniosek przyjęty!", result); // Tytuł
            Assert.Contains(name, result);
            Assert.Contains(bank, result);
            Assert.Contains(expectedCurrency, result);
            Assert.Contains("Loan Hub", result); // Stopka/Treść ogólna
        }

        [Fact]
        public void GetNewApplicationAlert_ShouldFormatDateCorrectly() {
            // Arrange
            var appId = 55;
            var name = "Anna Nowak";
            var date = new DateTime(2023, 10, 15, 14, 30, 0);
            var expectedDateString = "2023-10-15 14:30";

            // Act
            var result = _service.GetNewApplicationAlert(appId, name, date);

            // Assert
            Assert.Contains("Nowy wniosek do rozpatrzenia", result);
            Assert.Contains($"#{appId}", result);
            Assert.Contains(name, result);
            Assert.Contains(expectedDateString, result);
        }

        [Fact]
        public void GetOfferAcceptedWithDocs_ShouldContainLink() {
            // Arrange
            var name = "Piotr Zieliński";
            var link = "https://loanhub.com/sign/123";

            // Act
            var result = _service.GetOfferAcceptedWithDocs(name, link);

            // Assert
            Assert.Contains("Twój wniosek został zaakceptowany", result);
            Assert.Contains(name, result);
            Assert.Contains($"href='{link}'", result); // Sprawdzenie czy link jest w tagu href
        }

        [Fact]
        public void GetSignedDocsAlert_ShouldContainAppIdAndName() {
            // Arrange
            var appId = 999;
            var name = "Maria Wiśniewska";

            // Act
            var result = _service.GetSignedDocsAlert(appId, name);

            // Assert
            Assert.Contains("Klient podpisał umowę", result);
            Assert.Contains(name, result);
            Assert.Contains($"#{appId}", result);
        }

        [Fact]
        public void GetFinalApproval_ShouldDisplaySuccessMessageAndAmount() {
            // Arrange
            var name = "Tomek K";
            var amount = 50000m;
            var expectedCurrency = amount.ToString("C", new CultureInfo("pl-PL"));

            // Act
            var result = _service.GetFinalApproval(name, amount);

            // Assert
            Assert.Contains("Gratulacje! Pożyczka przyznana", result);
            Assert.Contains("Weryfikacja zakończona sukcesem", result);
            Assert.Contains(expectedCurrency, result);
        }

        [Fact]
        public void GetRejection_ShouldDisplayReason() {
            // Arrange
            var name = "Adam Pechowy";
            var reason = "Zbyt niska zdolność kredytowa";

            // Act
            var result = _service.GetRejection(name, reason);

            // Assert
            Assert.Contains("Status Twojego wniosku", result);
            Assert.Contains("nie mógł zostać zaakceptowany", result);
            Assert.Contains(reason, result);
        }

        [Fact]
        public void GetBaseHtml_ShouldIncludeFooterWithCurrentYear() {
            // Testuje czy metoda prywatna GetBaseHtml (wywoływana przez metody publiczne)
            // poprawnie generuje wspólne elementy, np. stopkę.

            // Arrange
            var currentYear = DateTime.Now.Year.ToString();

            // Act
            var result = _service.GetSubmissionConfirmation("Test", "Test", 0);

            // Assert
            Assert.Contains("&copy;", result);
            Assert.Contains(currentYear, result);
            Assert.Contains("Loan Hub", result);
        }
    }
}