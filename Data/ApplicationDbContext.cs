using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSets principales
        public DbSet<Tamal> Tamale { get; set; }
        public DbSet<ProductoBase> ProductBase {get; set;}
        public DbSet<Bebida> Beverage { get; set; }
        public DbSet<Combo> Combo { get; set; }
        public DbSet<ProductoCombo> ComboProduct { get; set; }
        public DbSet<Pedido> Order { get; set; }
        public DbSet<DetallePedido> OrderDetail { get; set; }
        public DbSet<Venta> Sales { get; set; }
        public DbSet<DetalleVenta> SalesDetail { get; set; }
        public DbSet<Usuario> User { get; set; }
        public DbSet<Sucursal> Branch { get; set; }
        public DbSet<MateriaPrima> RawMaterial { get; set; }
        public DbSet<InventarioItem> Inventory { get; set; }
        public DbSet<MovimientoInventario> InventoryMovement { get; set; }
        public DbSet<ComandoVoz> VoiceCommand { get; set; }
        public DbSet<Proveedor> Supplier { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Evitando borrado en cascada peligroso
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.Restrict;

            // Configuración ProductoCombo (relación N:N entre Combo y ProductoBase)
            modelBuilder.Entity<ProductoCombo>()
                .HasOne<Combo>()
                .WithMany(c => c.Products)
                .HasForeignKey(pc => pc.ComboId);

            // Relación Usuario-Pedido
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Usuario-Venta
            modelBuilder.Entity<Venta>()
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices para rendimiento en búsquedas comunes
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<ProductoBase>()
                .HasIndex(p => p.Name);

            // Precision para precios
            modelBuilder.Entity<ProductoBase>()
                .Property(p => p.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<DetalleVenta>()
                .Property(d => d.UnitPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<DetallePedido>()
                .Property(d => d.UnitPrice)
                .HasPrecision(10, 2);
        }
    }
}
