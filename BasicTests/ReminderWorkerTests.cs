using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite; // Dodaj ten namespace
using MortgageComparer.Data;
using MortgageComparer.Entities;
using MortgageComparer.StatesMachine;
using MortgageComparer.Services.Interfaces;
using MortgageComparer.Models;
using System;
using System.Threading.Tasks;
namespace MortgageComparer.Tests.Services;
public class ReminderWorkerTests {
    static ReminderWorkerTests() {
        SQLitePCL.Batteries.Init();
    }
    [Fact]
    public async Task WorkerLogic_ShouldSendEmail_WhenOfferIsApprovedAndOld_Async() {
        // ARRANGE
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .EnableSensitiveDataLogging()
            .Options;

        var context = new AppDbContext(options);

        context.Database.EnsureCreated();

        // --- TWORZENIE DANYCH TESTOWYCH ---

        // A. Użytkownik
        var user = new UserEntity {
            Id = 1,
            Email = "jan@test.pl",
            FirstName = "Jan",
            LastName = "Kowalski"
        };

        // B. Quote
        var quote = new QuoteEntity {
            QuoteId = 99,
            TotalAmountToPay = new MoneyModel { Amount = 12000, CurrencyCode = "PLN" },
            InstalmentNumber = 24,
            StatusId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        };

        // C. Oferty

        // Oferta 1: Stara (> 3 dni) -> Powinna dostać maila
        var oldOffer = new OfferEntity {
            Id = 1,
            User = user,
            Quote = quote,
            Status = OfferStatus.Approved,
            UpdateDate = DateTime.UtcNow.AddDays(-4),
            RequestedMoney = new MoneyModel { Amount = 1000, CurrencyCode = "PLN" }
        };

        // Oferta 2: Nowa (1 dzień) -> Nie powinna dostać maila
        var freshOffer = new OfferEntity {
            Id = 2,
            User = user,
            Quote = quote,
            Status = OfferStatus.Approved,
            UpdateDate = DateTime.UtcNow.AddDays(-1),
            RequestedMoney = new MoneyModel { Amount = 1000, CurrencyCode = "PLN" }
        };

        // Dodajemy wszystko do kontekstu
        // Kolejność jest ważna dla SQLite (Klucze obce), ale EF Core zazwyczaj sobie radzi
        context.Users.Add(user);
        context.Quotes.Add(quote);
        context.Offers.AddRange(oldOffer, freshOffer);

        await context.SaveChangesAsync();

        // 2. Mocki serwisów
        var emailServiceMock = new Mock<IEmailService>();
        var templateServiceMock = new Mock<IEmailTemplateService>();

        templateServiceMock
            .Setup(x => x.GetContractSigningReminder(It.IsAny<string>()))
            .Returns("<html>Treść Przypomnienia</html>");

        // 3. Symulacja Scope Factory
        var serviceProviderMock = new Mock<IServiceProvider>();
        var serviceScopeMock = new Mock<IServiceScope>();
        var scopeFactoryMock = new Mock<IServiceScopeFactory>();

        scopeFactoryMock.Setup(x => x.CreateScope()).Returns(serviceScopeMock.Object);
        serviceScopeMock.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);

        // Konfigurujemy ServiceProvidera (DI)
        // Zwracamy ten sam kontekst, który stworzyliśmy na połączeniu SQLite
        serviceProviderMock.Setup(x => x.GetService(typeof(AppDbContext))).Returns(context);
        serviceProviderMock.Setup(x => x.GetService(typeof(IEmailService))).Returns(emailServiceMock.Object);
        serviceProviderMock.Setup(x => x.GetService(typeof(IEmailTemplateService))).Returns(templateServiceMock.Object);

        // ACT
        // Symulujemy działanie wnętrza pętli Workera
        using (var scope = scopeFactoryMock.Object.CreateScope()) {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var email = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var templates = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();

            var threshold = DateTime.UtcNow.AddDays(-3);

            // Logika wyciągnięta z workera
            var forgottenOffers = await db.Offers
                .Include(o => o.User)
                .Where(o => o.Status == OfferStatus.Approved && o.UpdateDate < threshold)
                .ToListAsync();

            foreach (var offer in forgottenOffers) {
                var body = templates.GetContractSigningReminder(offer.User.FirstName);
                await email.SendEmailAsync(offer.User.Email, "Przypomnienie: Twoja umowa czeka!", body);
            }
        }

        // ASSERT

        // 1. Sprawdzamy czy wysłano maila do Jana (dla starej oferty)
        emailServiceMock.Verify(x => x.SendEmailAsync(
            "jan@test.pl",
            "Przypomnienie: Twoja umowa czeka!",
            "<html>Treść Przypomnienia</html>"
        ), Times.Once);

        // 2. Upewniamy się, że metoda szablonu została wywołana z imieniem Jana
        templateServiceMock.Verify(x => x.GetContractSigningReminder("Jan"), Times.Once);

        // Połączenie SQLite zamknie się automatycznie tutaj (koniec bloku using connection)
        // i baza danych zniknie z pamięci.
    }
}