using envolti.lib.order.domain.Order.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace envolti.lib.data.sqlserver.Order
{
    public class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
    {
        public void Configure( EntityTypeBuilder<OrderEntity> builder )
        {
            builder.ToTable( "Orders" );
            builder.HasKey( o => o.Id );
            builder.Property( o => o.OrderIdExternal ).IsRequired( );
            builder.Property( o => o.TotalPrice ).HasColumnType( "decimal(18,2)" );
            builder.Property( o => o.CreatedAt ).IsRequired( );
            builder.Property( o => o.Status ).IsRequired( );

            //builder.HasMany( o => o.Products )
            //       .WithOne( )
            //       .HasForeignKey( p => p.OrderId );
        }
    }
}
