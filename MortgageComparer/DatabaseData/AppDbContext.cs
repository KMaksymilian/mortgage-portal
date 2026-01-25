using Microsoft.EntityFrameworkCore;
using MortgageComparer.Entities;
using MortgageComparer.Models;

namespace MortgageComparer.Data;

public class AppDbContext : DbContext
{
    

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
    }
   
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<OfferEntity> Offers { get; set; }
    public DbSet<JobTypeEntity> JobTypes { get; set; }
    public DbSet<PersonalDocumentTypeEntity> DocumentTypes { get; set; }
    public DbSet<QuoteEntity> Quotes { get; set; }
    public DbSet<ApiUserEntity> OurApiUsers { get; set; }
    public DbSet<QuoteToBankEntity> QuoteToBanks { get; set; }
    public DbSet<OfferToBankEntity> OfferToBanks { get; set; }
}