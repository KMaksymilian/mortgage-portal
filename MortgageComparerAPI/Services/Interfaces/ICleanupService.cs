namespace MortgageComparerAPI.Services.Interfaces {
    public interface ICleanupService {
        Task ProcessExpiredOffersAsync(CancellationToken stoppingToken);
        Task ProcessOldQuotesAsync(CancellationToken stoppingToken);
    }
}