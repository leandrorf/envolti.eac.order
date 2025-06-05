using envolti.lib.data.sqlserver.Order;
using envolti.lib.order.domain.Order.Entities;
using Microsoft.EntityFrameworkCore;

namespace envolti.lib.data.sqlserver
{
    public class SqlServerDbContext : DbContext
    {
        public virtual DbSet<OrderEntity> Orders { get; set; }
        public virtual DbSet<ProductEntity> Products { get; set; }

        public SqlServerDbContext( DbContextOptions<SqlServerDbContext> options )
            : base( options )
        {
        }

        protected override void OnModelCreating( ModelBuilder modelBuilder )
        {
            modelBuilder.ApplyConfiguration( new OrderConfiguration( ) );
            //base.OnModelCreating( modelBuilder );
        }
    }
}
