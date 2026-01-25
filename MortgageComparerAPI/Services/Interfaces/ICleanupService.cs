namespace MortgageComparer.Services.BackgroundLogic {
    public interface ICleanupService {
        Task ProcessExpiredOffersAsync(CancellationToken stoppingToken);
        Task ProcessOldQuotesAsync(CancellationToken stoppingToken);
    }
}