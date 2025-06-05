using envolti.lib.order.domain.Order.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace envolti.lib.data.sqlserver.Product
{
    public class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
    {
        public void Configure( EntityTypeBuilder<ProductEntity> builder )
        {
            // Configure the ProductEntity properties and relationships here
            builder.ToTable( "Products" );
            builder.HasKey( p => p.Id );
            builder.Property( p => p.ProductIdExternal ).IsRequired( );
            builder.Property( p => p.Name ).IsRequired( ).HasMaxLength( 100 );
            builder.Property( p => p.Price ).HasColumnType( "decimal(18,2)" );
        }
    }
}
