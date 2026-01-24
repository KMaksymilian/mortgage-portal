using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MortgageComparer.StatesMachine;
using MortgageComparer.Services.Interfaces;
using MortgageComparer.Data;

public class ReminderWorker : BackgroundService {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _period = TimeSpan.FromHours(24); 

    public ReminderWorker(IServiceScopeFactory scopeFactory) {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        // PeriodicTimer to nowoczesny sposób na pętle w workerach
        using PeriodicTimer timer = new PeriodicTimer(_period);

        // Pętla wykonuje się dopóki nie zatrzymasz aplikacji
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken)) {
            try {
                using (var scope = _scopeFactory.CreateScope()) {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    var templateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();

                    var threshold = DateTime.UtcNow.AddDays(-3);

                    // --- Logika dla Użytkownika (Approved = czeka na podpis) ---
                    var forgottenOffers = await context.Offers
                        .Include(o => o.User)
                        .Where(o => o.Status == OfferStatus.Approved && o.UpdatedAt < threshold)
                        .ToListAsync(stoppingToken);

                    foreach (var offer in forgottenOffers) {
                        if (offer.User == null) {
                            continue;
                        }

                    
                        var emailBody = templateService.GetContractSigningReminder(offer.User.FirstName);

                        // 3. Wysyłamy
                        await emailService.SendEmailAsync(
                            offer.User.Email,
                            "Przypomnienie: Twoja umowa czeka!",
                            emailBody
                        );
                    }

    
                    var stalledOffersBank = await context.Offers
                        .Include(o => o.User)
                        .Where(o => (o.Status == OfferStatus.Pending || o.Status == OfferStatus.ContractSigned)
                                    && o.UpdatedAt < threshold)
                        .ToListAsync(stoppingToken);

                    foreach (var offer in stalledOffersBank) {
                        var adminBody = templateService.GetNewApplicationAlert(offer.Id, offer.User.FirstName, offer.CreatedAt);
                        await emailService.SendEmailAsync("admin@bank.pl", "Wniosek czeka na decyzję!", adminBody);
                    }
                }
            }
            catch (Exception ex) {
                   
            }
        }
    }
}