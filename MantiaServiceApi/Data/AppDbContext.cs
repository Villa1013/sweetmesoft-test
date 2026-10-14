using MantiaServiceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MantiaServiceAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Solicitud> Solicitudes { get; set; }
    public DbSet<ItemSolicitud> ItemsSolicitud { get; set; }
    public DbSet<Tarifa> Tarifas { get; set; }
    public DbSet<Cotizacion> Cotizaciones { get; set; }
    public DbSet<LineaCotizacion> LineasCotizacion { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Solicitud>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Numero).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Numero).IsUnique();
            entity.Property(e => e.Cliente).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Contacto).HasMaxLength(200);
            entity.Property(e => e.Municipio).HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(500);
            entity.Property(e => e.EmailContacto).HasMaxLength(200);
            entity.Property(e => e.TelefonoContacto).HasMaxLength(50);
            entity.Property(e => e.Referencia).HasMaxLength(100);
            entity.Property(e => e.FranjaHoraria).HasMaxLength(100);
            entity.Property(e => e.TextoOriginal).HasMaxLength(16000);
            entity.Property(e => e.Observaciones).HasMaxLength(2000);
            entity.Property(e => e.CamposFaltantes).HasMaxLength(1000);
        });

        modelBuilder.Entity<ItemSolicitud>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CodigoTarifa).HasMaxLength(20);
            entity.Property(e => e.Concepto).HasMaxLength(300);
            entity.Property(e => e.Observacion).HasMaxLength(500);
            entity.Property(e => e.Cantidad).HasPrecision(18, 2);
            entity.HasOne(e => e.Solicitud)
                .WithMany(s => s.Items)
                .HasForeignKey(e => e.SolicitudId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Tarifa>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Version).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Concepto).HasMaxLength(300);
            entity.Property(e => e.Unidad).HasMaxLength(50);
            entity.Property(e => e.PrecioUnitarioCOP).HasPrecision(18, 2);
            entity.HasIndex(e => new { e.Version, e.Codigo }).IsUnique();
        });

        modelBuilder.Entity<Cotizacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Numero).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Numero).IsUnique();
            entity.Property(e => e.Cliente).HasMaxLength(200);
            entity.Property(e => e.Municipio).HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(500);
            entity.Property(e => e.VersionTarifa).HasMaxLength(10);
            entity.Property(e => e.Observaciones).HasMaxLength(2000);
            entity.Property(e => e.Subtotal).HasPrecision(18, 2);
            entity.Property(e => e.IVA).HasPrecision(18, 2);
            entity.Property(e => e.Total).HasPrecision(18, 2);
            entity.Property(e => e.PorcentajeIVA).HasPrecision(5, 2);
            entity.HasOne(e => e.Solicitud)
                .WithMany(s => s.Cotizaciones)
                .HasForeignKey(e => e.SolicitudId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        });

        modelBuilder.Entity<LineaCotizacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Codigo).HasMaxLength(20);
            entity.Property(e => e.Concepto).HasMaxLength(300);
            entity.Property(e => e.PrecioUnitarioCOP).HasPrecision(18, 2);
            entity.Property(e => e.ValorLineaCOP).HasPrecision(18, 2);
            entity.Property(e => e.Cantidad).HasPrecision(18, 2);
            entity.HasOne(e => e.Cotizacion)
                .WithMany(c => c.Lineas)
                .HasForeignKey(e => e.CotizacionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
