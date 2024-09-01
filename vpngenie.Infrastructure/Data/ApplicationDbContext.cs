using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using vpngenie.Domain.Entities;
using vpngenie.Infrastructure.Configurations;

namespace vpngenie.Infrastructure.Data;

public class ApplicationDbContext(IConfiguration configuration) : DbContext
{
    public DbSet<User> Users { get; init; }
    public DbSet<Promotion> Promotions { get; init; }
    public DbSet<Promocode> Promocodes { get; init; }
    public DbSet<PaymentHistory> PaymentHistories { get; init; }
    public DbSet<Ticket> Tickets { get; init; }
    public DbSet<Server> Servers { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var conf = configuration.GetConnectionString(nameof(ApplicationDbContext));
        optionsBuilder.UseNpgsql(conf);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new TicketConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentHistoryConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}