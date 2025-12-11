using Microsoft.EntityFrameworkCore;
using MortgageComparer.Models;

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
    public DbSet<User> Users { get; set; }
}