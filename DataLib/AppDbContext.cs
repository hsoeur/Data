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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Let EF Core infer the model from data annotations in ManufacturingOrder
            base.OnModelCreating(modelBuilder);
        }
    }
}
