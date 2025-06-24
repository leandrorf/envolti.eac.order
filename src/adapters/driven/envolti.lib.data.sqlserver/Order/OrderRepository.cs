using envolti.lib.order.domain.Order.Dtos;
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

            return await strategy.ExecuteAsync( async ( ) =>
            {
                await using var transaction = await _DbContext.Database.BeginTransactionAsync( );

                try
                {
                    _DbContext.Orders.Add( order );
                    await _DbContext.SaveChangesAsync( );
                    await transaction.CommitAsync( );
                    return order;
                }
                catch ( DbUpdateException ex )
                {
                    await transaction.RollbackAsync( );
                    throw new Exception( "Erro ao criar pedido - possível falha de conexão", ex );
                }

                catch ( Exception ex )
                {
                    await transaction.RollbackAsync( );
                    throw new Exception( "Erro ao criar pedido", ex );
                }
            } );

        }

        public async Task<PagedResult<OrderEntity>> GetAllAsync( int pageNumber = 1, int pageSize = 10 )
        {

            return null;
            //return await _DbContext.Orders
            //    .Include( o => o.Products )
            //    .AsNoTracking( )
            //    .ToListAsync( );
        }

        public async Task<OrderEntity?> GetOrderByIdAsync( int orderIdExternal )
        {
            return await _DbContext.Orders
                .Include( o => o.Products )
                .AsNoTracking( )
                .FirstOrDefaultAsync( x => x.OrderIdExternal == orderIdExternal );
        }

        public async Task<PagedResult<OrderEntity>> GetOrdersByStatusAsync( StatusEnum status, int pageNumber, int pageSize )
        {
            return null;
            //var filter = Builders<OrderEntity>.Filter.Eq( o => o.Status, status );
            //var total = ( int )await _collection.CountDocumentsAsync( filter );

            //var pedidos = await _collection.Find( filter )
            //    .Skip( ( pageNumber - 1 ) * pageSize )
            //    .Limit( pageSize )
            //    .ToListAsync( );

            //return new PagedResult<OrderEntity>
            //{
            //    Total = total,
            //    PaginaAtual = pageNumber,
            //    TamanhoPagina = pageSize,
            //    Itens = pedidos
            //};

        }

        public async Task<bool> OrderExistsAsync( int id )
        {
            return await _DbContext.Orders
                .AsNoTracking( )
                .Select( x => x.OrderIdExternal )
                .AnyAsync( x => x == id );
        }
    }
}
