using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MortgageComparer.Data;
using MortgageComparer.Entities;
using MortgageComparer.StatesMachine;


namespace MortgageComparer.Services.BackgroundLogic {
    public class CleanupService : ICleanupService {
        private readonly AppDbContext _context;
        private readonly ILogger<CleanupService> _logger;
        private readonly int _expirationThreshold = 10;

        public CleanupService(AppDbContext context, ILogger<CleanupService> logger) {
            _context = context;
            _logger = logger;
        }

        // --- LOGIKA DLA OFERT (10 Dni - Delete) ---
        public async Task ProcessExpiredOffersAsync(CancellationToken stoppingToken) {
            var expirationThreshold = DateTime.UtcNow.AddDays(-_expirationThreshold);

            int deletedCount = await _context.Offers
                .Where(o => o.UpdateDate < expirationThreshold
                        && (o.Status == OfferStatus.Rejected || o.Status == OfferStatus.Canceled))
                .ExecuteDeleteAsync(stoppingToken);

            if (deletedCount > 0) {
                _logger.LogInformation($"[Cleanup] Trwale usunięto {deletedCount} przeterminowanych ofert.");
            }
        }

        // --- LOGIKA DLA QUOTES (10 Dni - Delete) ---
        public async Task ProcessOldQuotesAsync(CancellationToken stoppingToken) {
            var deleteThreshold = DateTime.UtcNow.AddDays(-_expirationThreshold);

            int deletedCount = await _context.Quotes
                .Where(q => q.CreatedAt < deleteThreshold)
                .Where(q => !_context.Offers.Any(o => o.QuoteId == q.QuoteId))
                .ExecuteDeleteAsync(stoppingToken);

            if (deletedCount > 0) {
                _logger.LogInformation($"[Cleanup] Trwale usunięto {deletedCount} starych zapytań (Quotes), które nie miały ofert.");
            }
        }
    }
}