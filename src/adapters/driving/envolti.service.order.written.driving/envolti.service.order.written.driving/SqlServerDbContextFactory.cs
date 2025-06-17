using envolti.lib.data.sqlserver;
using envolti.lib.order.domain.Order.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace envolti.service.order.written.driving
{
    public class SqlServerDbContextFactory : IDesignTimeDbContextFactory<SqlServerDbContext>
    {
        public SqlServerDbContext CreateDbContext( string[ ] args )
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlServerDbContext>( );
            optionsBuilder.UseSqlServer(
                "Server=localhost;User ID=SA;Password=L3@ndr0rf;Database=OrderManagemnt;TrustServerCertificate=True;"
            );

            // Cria instâncias manuais dos serviços necessários
            var loggerFactory = LoggerFactory.Create( builder => builder.AddConsole( ) );
            var logger = loggerFactory.CreateLogger<SqlServerDbContext>( );

            var settings = Options.Create( new SqlServerSettings
            {
                Default = "Server=localhost;User ID=SA;Password=L3@ndr0rf;Database=OrderManagemnt;TrustServerCertificate=True;"
            } );

            return new SqlServerDbContext( optionsBuilder.Options, logger, settings );
        }
    }
}
