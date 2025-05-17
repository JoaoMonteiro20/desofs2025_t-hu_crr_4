using Microsoft.EntityFrameworkCore;
using EcoImpact.DataModel.Models;

namespace EcoImpact.DataModel;

public class EcoDbContext : DbContext
{
    public EcoDbContext(DbContextOptions<EcoDbContext> options) : base(options) { }
    public EcoDbContext() { }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<HabitType> HabitTypes { get; set; }
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

        modelBuilder.Entity<HabitType>().HasData(
    new HabitType { HabitTypeId = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Deslocação de carro (gasolina)", Unit = "km", Factor = 0.192M },
    new HabitType { HabitTypeId = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Viagem de comboio", Unit = "km", Factor = 0.041M },
    new HabitType { HabitTypeId = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Viagem de avião", Unit = "km", Factor = 0.255M },
    new HabitType { HabitTypeId = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Consumo de eletricidade", Unit = "kWh", Factor = 0.233M },
    new HabitType { HabitTypeId = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Refeição com carne", Unit = "unidade", Factor = 5.0M },
    new HabitType { HabitTypeId = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "Refeição vegetariana", Unit = "unidade", Factor = 2.0M },
    new HabitType { HabitTypeId = Guid.Parse("77777777-7777-7777-7777-777777777777"), Name = "Compra de bens eletrónicos", Unit = "unidade", Factor = 350M },
    new HabitType { HabitTypeId = Guid.Parse("88888888-8888-8888-8888-888888888888"), Name = "Banho quente (10 min)", Unit = "minuto", Factor = 0.3M },
    new HabitType { HabitTypeId = Guid.Parse("99999999-9999-9999-9999-999999999999"), Name = "Reciclagem de plástico", Unit = "kg", Factor = -1.8M },
    new HabitType { HabitTypeId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Plantação de árvores", Unit = "unidade", Factor = -21M }
);
    }
}
