using envolti.lib.data.sqlserver.Order;
using envolti.lib.data.sqlserver.Product;
using envolti.lib.order.domain.Order.Entities;
using envolti.lib.order.domain.Order.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace envolti.lib.data.sqlserver
{
    public class SqlServerDbContext : DbContext
    {
        public virtual DbSet<OrderEntity> Orders { get; set; }
        public virtual DbSet<ProductEntity> Products { get; set; }
        private readonly ILogger<SqlServerDbContext> _Logger;
        private readonly IOptions<SqlServerSettings> _Settings;

        public SqlServerDbContext( DbContextOptions<SqlServerDbContext> options, ILogger<SqlServerDbContext> logger, IOptions<SqlServerSettings> settings )
            : base( options )
        {
            _Settings = settings ?? throw new ArgumentNullException( nameof( settings ) );
            _Logger = logger ?? throw new ArgumentNullException( nameof( logger ) );
        }

        protected override void OnModelCreating( ModelBuilder modelBuilder )
        {
            modelBuilder.ApplyConfiguration( new OrderConfiguration( ) );
            modelBuilder.ApplyConfiguration( new ProductConfiguration( ) );
            base.OnModelCreating( modelBuilder );
        }
    }
}
