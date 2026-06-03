using System;
using Microsoft.EntityFrameworkCore;

namespace DataLib
{
    /// <summary>
    /// EF Core DbContext for domain models defined in DataLib.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ManufacturingOrder> ManufacturingOrders { get; set; }
        public DbSet<ManufacturingOrderNotification> ManufacturingOrderNotifications { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<BillOfMaterial> BillOfMaterials { get; set; }
        public DbSet<FirmwareVersion> FirmwareVersions { get; set; }
        public DbSet<Firmware> Firmwares { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<Sequence> Sequences { get; set; }
        public DbSet<ProductTraceability> ProductTraceabilities { get; set; }
        public DbSet<ProductStatus> ProductStatuses { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<LabelType> LabelTypes { get; set; }
        public DbSet<Tolerance> Tolerances { get; set; }
        public DbSet<LabelTemplate> LabelTemplates { get; set; }
        public DbSet<SolderingProgram> SolderingPrograms { get; set; }
        public DbSet<SimCard> SimCards { get; set; }
        public DbSet<PackagingLocationPhoto> PackagingLocationPhotos { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<DatabaseNotification> DatabaseNotifications { get; set; }
        public DbSet<KitTraceability> KitTraceabilities { get; set; }
        public DbSet<KitAccTraceability> KitAccTraceabilities { get; set; }
        public DbSet<MacAddress> MacAddresses { get; set; }
        public DbSet<DeviceIdentity> DeviceIdentities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Let EF Core infer the model from data annotations in ManufacturingOrder
            base.OnModelCreating(modelBuilder);

            // Configure unique constraints for DeviceIdentity
            modelBuilder.Entity<DeviceIdentity>()
                .HasIndex(d => d.EquipSN)
                .IsUnique();

            modelBuilder.Entity<DeviceIdentity>()
                .HasIndex(d => d.MacAddressValue)
                .IsUnique();

            // Disable cascade delete globally to avoid unintended deletions during GenDb updates
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var fk in entityType.GetForeignKeys())
                {
                    if (fk.DeleteBehavior == DeleteBehavior.Cascade)
                    {
                        fk.DeleteBehavior = DeleteBehavior.Restrict;
                    }
                }
            }
        }
    }
}
