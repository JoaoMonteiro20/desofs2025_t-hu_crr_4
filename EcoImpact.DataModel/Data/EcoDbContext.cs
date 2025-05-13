using Microsoft.EntityFrameworkCore;
using EcoImpact.DataModel.Models;

namespace EcoImpact.DataModel;

public class EcoDbContext : DbContext
{
    public EcoDbContext(DbContextOptions<EcoDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<HabitType> HabitTypes { get; set; }
    public DbSet<UserChoice> UserChoices { get; set; }
    public DbSet<FootprintSummary> FootprintSummaries { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User → UserChoice (1:N)
        modelBuilder.Entity<User>()
            .HasMany(u => u.UserChoices)
            .WithOne(uc => uc.User)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // HabitType → UserChoice (1:N)
        modelBuilder.Entity<HabitType>()
            .HasMany(ht => ht.UserChoices)
            .WithOne(uc => uc.HabitType)
            .HasForeignKey(uc => uc.HabitTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        // User → FootprintSummary (1:N)
        modelBuilder.Entity<User>()
            .HasMany(u => u.Summaries)
            .WithOne(fs => fs.User)
            .HasForeignKey(fs => fs.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // User → AuditLog (1:N)
        modelBuilder.Entity<User>()
            .HasMany(u => u.AuditLogs)
            .WithOne(al => al.User)
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
