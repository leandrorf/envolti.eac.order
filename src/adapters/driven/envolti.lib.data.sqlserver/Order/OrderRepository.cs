using envolti.lib.order.domain.Order.Entities;
using envolti.lib.order.domain.Order.Enums;
using envolti.lib.order.domain.Order.Ports;
using Microsoft.EntityFrameworkCore;

namespace envolti.lib.data.sqlserver.Order
{
    public class OrderRepository : IOrderRepository
    {
        private readonly SqlServerDbContext _DbContext;

        public OrderRepository( SqlServerDbContext dbContext )
        {
            _DbContext = dbContext;
        }

        public async Task<OrderEntity> CreateOrderAsync( OrderEntity order )
        {
            var strategy = _DbContext.Database.CreateExecutionStrategy( );

            await strategy.ExecuteAsync( async ( ) =>
            {
                using ( var transaction = await _DbContext.Database.BeginTransactionAsync( ) )
                {
                    try
                    {
                        _DbContext.Orders.Add( order );
                        await _DbContext.SaveChangesAsync( );
                        await transaction.CommitAsync( );
                    }
                    catch
                    {
                        await transaction.RollbackAsync( );
                        throw;
                    }
                }
            } );

            return order;
        }

        public async Task<IEnumerable<OrderEntity>> GetAllAsync( )
        {
            return await _DbContext.Orders
                .AsNoTracking( )
                .Include( o => o.Products )
                .ToListAsync( );
        }

        public async Task<OrderEntity?> GetOrderByIdAsync( int orderIdExternal )
        {
            return await _DbContext.Orders
                .AsNoTracking( )
                .Include( o => o.Products )
                .FirstOrDefaultAsync( x => x.OrderIdExternal == orderIdExternal );
        }

        public async Task<IEnumerable<OrderEntity>> GetOrdersByStatusAsync( StatusEnum status )
        {
            return await _DbContext.Orders
                .AsNoTracking( )
                .Include( o => o.Products )
                .Where( x => x.Status == status )
                .ToListAsync( );
        }

        public async Task<bool> OrderExistsAsync( int id )
        {
            return await _DbContext.Orders.AnyAsync( x => x.OrderIdExternal == id );
        }
    }
}
