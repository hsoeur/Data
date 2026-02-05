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
        public DbSet<ArticleVersion> ArticleVersions { get; set; }
        public DbSet<Firmware> Firmwares { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<Sequence> Sequences { get; set; }
        public DbSet<MeasurementTraceability> MeasurementTraceabilities { get; set; }
        public DbSet<ProductTraceability> ProductTraceabilities { get; set; }
        public DbSet<ProductStatus> ProductStatuses { get; set; }
        public DbSet<LabelType> LabelTypes { get; set; }
        public DbSet<Tolerance> Tolerances { get; set; }
        public DbSet<LabelTemplate> LabelTemplates { get; set; }
        public DbSet<SolderingProgram> SolderingPrograms { get; set; }
        public DbSet<SimCard> SimCards { get; set; }
        public DbSet<PackagingLocationPhoto> PackagingLocationPhotos { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<DatabaseNotification> DatabaseNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Let EF Core infer the model from data annotations in ManufacturingOrder
            base.OnModelCreating(modelBuilder);

        
        }
    }
}
