using Microsoft.EntityFrameworkCore;
using MortgageComparer.Entities;

namespace MortgageComparer.Data;

public class AppDbContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public AppDbContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseNpgsql(Configuration.GetConnectionString("DefaultConnectionString"));
    }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<OfferEntity> Offers { get; set; }
    public DbSet<JobTypeEntity> JobTypes { get; set; }
    public DbSet<PersonalDocumentTypeEntity> DocumentTypes { get; set; }
}