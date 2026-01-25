using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MortgageComparer.Services.BackgroundLogic;

namespace MortgageComparer.Workers {
    public class CleanupWorker : BackgroundService {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CleanupWorker> _logger;
        private readonly TimeSpan _period = TimeSpan.FromHours(24);

        public CleanupWorker(IServiceScopeFactory scopeFactory, ILogger<CleanupWorker> logger) {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            _logger.LogInformation("Cleanup Worker uruchomiony.");

            using PeriodicTimer timer = new PeriodicTimer(_period);

            do {
                try {
                    using (var scope = _scopeFactory.CreateScope()) {
                        var cleanupService = scope.ServiceProvider.GetRequiredService<ICleanupService>();

                        _logger.LogInformation("Rozpoczynam cykl czyszczenia danych...");
                        await cleanupService.ProcessExpiredOffersAsync(stoppingToken);
                        await cleanupService.ProcessOldQuotesAsync(stoppingToken);
                        _logger.LogInformation("Zakończono cykl czyszczenia.");
                    }
                }
                catch (Exception ex) {
                    _logger.LogError(ex, "Błąd krytyczny w CleanupWorker.");
                }

            } while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken));
        }
    }
}