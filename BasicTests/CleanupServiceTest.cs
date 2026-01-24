using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
// Upewnij się, że te namespace'y pasują do Twojego projektu
using MortgageComparer.Data;
using MortgageComparer.Entities;
using MortgageComparer.Models;
using MortgageComparer.Services.BackgroundLogic;
using MortgageComparer.StatesMachine;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit; // To jest wymagane dla [Fact] i Assert

namespace BasicTests;
public class CleanupServiceTests {
    // Helper do tworzenia bazy In-Memory
    private AppDbContext GetInMemoryDbContext() {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .EnableSensitiveDataLogging()
            .Options;

        var context = new AppDbContext(options);

        context.Database.EnsureCreated();

        return context;
    }

    [Fact]
    public async Task ProcessExpiredOffers_ShouldDelete_OnlyRejectedOrCanceled_OlderThan10Days_Async() {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = Mock.Of<ILogger<CleanupService>>();
        var service = new CleanupService(context, logger);

        var oldDate = DateTime.UtcNow.AddDays(-15);
        var newDate = DateTime.UtcNow.AddDays(-5);

        var user = new UserEntity {
            FirstName = "Test",
            LastName = "User",
            Id = 1,
            Email = "test@test.pl"
        };

        var quote = new QuoteEntity {
            QuoteId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-20),
            TotalAmountToPay = new MoneyDto(5000, "PLN")
        };

        context.Users.Add(user);
        context.Quotes.Add(quote);
        await context.SaveChangesAsync();

        context.Offers.AddRange(new List<OfferEntity>
        {
            // Powinno zostać usunięte (Stare + Rejected)
            new OfferEntity {
                Id = 1,
                UserId = user.Id,
                QuoteId = quote.QuoteId,
                CreatedAt = oldDate,
                Status = OfferStatus.Rejected,
                RequestedMoney = new MoneyDto (1000, "PLN")
            },
            
            // Powinno zostać usunięte (Stare + Canceled)
            new OfferEntity {
                Id = 2,
                UserId = user.Id,
                QuoteId = quote.QuoteId,
                CreatedAt = oldDate,
                Status = OfferStatus.Canceled,
                RequestedMoney = new MoneyDto (1000, "PLN")
            },
            
            // Powinno zostać (Status Completed nie jest usuwalny w tej logice)
            new OfferEntity {
                Id = 3,
                UserId = user.Id,
                QuoteId = quote.QuoteId,
                CreatedAt = oldDate,
                Status = OfferStatus.Completed,
                RequestedMoney = new MoneyDto (1000, "PLN")
            },

            // Powinno zostać (Status Created - może zbyt wcześnie na usunięcie?)
            new OfferEntity {
                Id = 4,
                UserId = user.Id,
                QuoteId = quote.QuoteId,
                CreatedAt = oldDate,
                Status = OfferStatus.Created,
                RequestedMoney = new MoneyDto (1000, "PLN")
            },
            
            // Powinno zostać (Zbyt nowa data)
            new OfferEntity {
                Id = 5,
                UserId = user.Id,
                QuoteId = quote.QuoteId,
                CreatedAt = newDate,
                Status = OfferStatus.Rejected,
                RequestedMoney = new MoneyDto (1000, "PLN")
            }
        });
        await context.SaveChangesAsync();

        // Act
        await service.ProcessExpiredOffersAsync(CancellationToken.None);
        context.ChangeTracker.Clear();

        // Assert
        Assert.Null(await context.Offers.FindAsync(1)); // Usunięte
        Assert.Null(await context.Offers.FindAsync(2)); // Usunięte

        Assert.NotNull(await context.Offers.FindAsync(3)); // Zostało
        Assert.NotNull(await context.Offers.FindAsync(4)); // Zostało
        Assert.NotNull(await context.Offers.FindAsync(5)); // Zostało
    }

    [Fact]
    public async Task ProcessOldQuotes_ShouldDelete_OldQuotes_OnlyIfNoLinkedOffers_Async() {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = Mock.Of<ILogger<CleanupService>>();
        var service = new CleanupService(context, logger);

        var oldDate = DateTime.UtcNow.AddDays(-15);
        var newDate = DateTime.UtcNow.AddDays(-5);

        var quoteToDelete = new QuoteEntity {
            QuoteId = 1,
            CreatedAt = oldDate,
            TotalAmountToPay = new MoneyDto(1000, "PLN")
        };

        var quoteWithOffer = new QuoteEntity {
            QuoteId = 2,
            CreatedAt = oldDate,
            TotalAmountToPay = new MoneyDto(1000, "PLN")
        };

        var quoteNew = new QuoteEntity {
            QuoteId = 3,
            CreatedAt = newDate,
            TotalAmountToPay = new MoneyDto(1000, "PLN")
        };

        context.Quotes.AddRange(quoteToDelete, quoteWithOffer, quoteNew);

        var user = new UserEntity {
            FirstName = "Test",
            LastName = "User",
            Id = 1,
            Email = "test@test.pl"
        };
        context.Users.Add(user);

        context.Offers.Add(new OfferEntity {
            Id = 100,
            UserId = user.Id,
            QuoteId = 2,
            CreatedAt = DateTime.UtcNow,
            Status = OfferStatus.Created,
            RequestedMoney = new MoneyDto(1000, "PLN")
        });

        await context.SaveChangesAsync();

        // Act
        await service.ProcessOldQuotesAsync(CancellationToken.None);

        context.ChangeTracker.Clear();

        // Assert
        Assert.Null(await context.Quotes.FindAsync(1));    
        Assert.NotNull(await context.Quotes.FindAsync(2));  
        Assert.NotNull(await context.Quotes.FindAsync(3));  
    }
}