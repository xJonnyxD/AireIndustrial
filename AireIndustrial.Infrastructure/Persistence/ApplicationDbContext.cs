using AireIndustrial.Domain.Entities;
using AireIndustrial.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AireIndustrial.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<AppIdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<SensorCalidadAire> SensoresCalidadAire { get; set; }
    public DbSet<LecturaAire> LecturasAire { get; set; }
    public DbSet<AlertaAire> AlertasAire { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SensorCalidadAire>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Ubicacion).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TipoGas).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Estado).IsRequired().HasMaxLength(50);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<LecturaAire>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Sensor)
                  .WithMany(s => s.Lecturas)
                  .HasForeignKey(e => e.SensorId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<AlertaAire>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nivel).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Mensaje).IsRequired().HasMaxLength(500);
            entity.HasOne(e => e.Sensor)
                  .WithMany(s => s.Alertas)
                  .HasForeignKey(e => e.SensorId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }
}
