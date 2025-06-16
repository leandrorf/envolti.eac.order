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
        private readonly IOptions<SqlServerSettigns> _Settings;

        public SqlServerDbContext( DbContextOptions<SqlServerDbContext> options, ILogger<SqlServerDbContext> logger, IOptions<SqlServerSettigns> settings )
            : base( options )
        {            
            _Settings = settings ?? throw new ArgumentNullException( nameof( settings ) );
            _Logger = logger ?? throw new ArgumentNullException( nameof( logger ) );

            _Logger.LogInformation( "SqlServerDbContext initialized with connection string: {ConnectionString}", _Settings.Value.Default );
        }

        //protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
        //{
        //    _Logger.LogInformation( "Configuring SqlServerDbContext..." );
        //    if ( !optionsBuilder.IsConfigured )
        //    {
        //        _Logger.LogInformation( "Using default connection string from settings." );
        //        optionsBuilder.UseSqlServer( "Server=localhost;User ID=SA;Password=L3@ndr0rf;Database=OrderManagemnt;TrustServerCertificate=True;" )
        //                      .EnableSensitiveDataLogging( true )
        //                      .LogTo( msg => _Logger.LogInformation( msg ), LogLevel.Information );
        //    }

        //    base.OnConfiguring( optionsBuilder );
        //}

        protected override void OnModelCreating( ModelBuilder modelBuilder )
        {
            modelBuilder.ApplyConfiguration( new OrderConfiguration( ) );
            modelBuilder.ApplyConfiguration( new ProductConfiguration( ) );
            base.OnModelCreating( modelBuilder );
        }
    }
}
