using Microsoft.EntityFrameworkCore;
using EcoImpact.DataModel.Models;
using System.Data;

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


        modelBuilder.Entity<User>().HasData(
    new User
    {
        UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // ou outro GUID fixo
        UserName = "admin",
        Email = "admin@ecoimpact.local",
        Password = "AQAAAAIAAYagAAAAEEISJn23wqjANxH/pmq3ug2f+MTVEF+p5yB7TORYNv6wFmeRVaTTL1G1objmD/A9Dg==",
        Role = UserRole.Admin
    });
        modelBuilder.Entity<HabitType>().HasData(
        // Transporte
        new HabitType
        {
            HabitTypeId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Deslocação de carro (gasolina)",
            Unit = "km",
            Factor = 0.192M,
            Category = HabitCategory.Transporte
        },
        new HabitType
        {
            HabitTypeId = Guid.Parse("11111111-1111-1111-1111-111111111112"),
            Name = "Viagem de comboio",
            Unit = "km",
            Factor = 0.041M,
            Category = HabitCategory.Transporte
        },
        new HabitType
        {
            HabitTypeId = Guid.Parse("11111111-1111-1111-1111-111111111113"),
            Name = "Viagem de avião",
            Unit = "km",
            Factor = 0.255M,
            Category = HabitCategory.Transporte
        },

        // Alimentação
        new HabitType
        {
            HabitTypeId = Guid.Parse("22222222-2222-2222-2222-222222222221"),
            Name = "Refeição com carne",
            Unit = "unidade",
            Factor = 5.0M,
            Category = HabitCategory.Alimentacao
        },
        new HabitType
        {
            HabitTypeId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "Refeição vegetariana",
            Unit = "unidade",
            Factor = 2.0M,
            Category = HabitCategory.Alimentacao
        },
        new HabitType
        {
            HabitTypeId = Guid.Parse("22222222-2222-2222-2222-222222222223"),
            Name = "Refeição vegan",
            Unit = "unidade",
            Factor = 1.5M,
            Category = HabitCategory.Alimentacao
        },

        // Energia
        new HabitType
        {
            HabitTypeId = Guid.Parse("33333333-3333-3333-3333-333333333331"),
            Name = "Consumo de eletricidade",
            Unit = "kWh",
            Factor = 0.233M,
            Category = HabitCategory.Energia
        },
        new HabitType
        {
            HabitTypeId = Guid.Parse("33333333-3333-3333-3333-333333333332"),
            Name = "Banho quente (10 min)",
            Unit = "minuto",
            Factor = 0.3M,
            Category = HabitCategory.Energia
        },
        new HabitType
        {
            HabitTypeId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Name = "Uso de aquecedor elétrico",
            Unit = "hora",
            Factor = 1.2M,
            Category = HabitCategory.Energia
        }
    );
    }
    }
