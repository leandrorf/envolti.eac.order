using envolti.lib.order.application.Mediator.Interfaces;
using envolti.lib.order.application.Order.Queries;
using envolti.lib.order.application.Order.Responses;
using envolti.lib.order.domain.Order.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace envolti.api.order.reading.driving.Controllers
{
    [Route( "api/[controller]" )]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrdersController> _Logger;

        public OrdersController( IMediator mediator, ILogger<OrdersController> logger )
        {
            _mediator = mediator;
            _Logger = logger;
        }

        [HttpGet( "{OrderIdExternal}" )]
        public async Task<ActionResult<OrderListResponse>> Get( int OrderIdExternal )
        {
            Stopwatch stopwatch = Stopwatch.StartNew( );

            var query = new GetOrderByIdQuery
            {
                OrderIdExternal = OrderIdExternal
            };

            var response = await _mediator.Send( query );

            if ( response.Success )
            {
                stopwatch.Stop( );
                _Logger.LogInformation( $"Tempo total da requisição do pedido por Id: {stopwatch.ElapsedMilliseconds} ms" );
                Console.WriteLine( $"Tempo total da requisição do pedido por Id: {stopwatch.ElapsedMilliseconds} ms" );

                return Ok( response.Data );
            }
            else if ( response.ErrorCode == ErrorCodesResponseEnum.RECORD_NOT_FOUND )
            {
                _Logger.LogWarning( "Pedido não encontrado." );
                return NotFound( response );
            }

            _Logger.LogError( "An unexpected error occurred: {Message}", response.Message );
            return BadRequest( response );
        }

        [HttpGet( "GetAll" )]
        public async Task<ActionResult<OrderListResponse>> GetAll( int pageNumber = 1, int pageSize = 10 )
        {
            Stopwatch stopwatch = Stopwatch.StartNew( );

            var query = new GetAllOrdersQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var response = await _mediator.Send( query );

            if ( response.Data?.Items != null && response.Data.Items.Any( ) )
            {
                stopwatch.Stop( );
                _Logger.LogInformation( $"Tempo total da requisição por todos os pedidos: {stopwatch.ElapsedMilliseconds} ms" );
                Console.WriteLine( $"Tempo total da requisição por todos os pedidos: {stopwatch.ElapsedMilliseconds} ms" );

                return Ok( response );
            }

            return NotFound( );
        }

        [HttpGet( "GetByStatus" )]
        public async Task<ActionResult<OrderListResponse>> GetByStatus( StatusEnum status, int pageNumber = 1, int pageSize = 10 )
        {
            Stopwatch stopwatch = Stopwatch.StartNew( );

            var query = new GetOrdersByStatusQuery
            {
                Status = status,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var response = await _mediator.Send( query );

            if ( response.Data?.Items != null && response.Data.Items.Any( ) )
            {
                stopwatch.Stop( );
                _Logger.LogInformation( $"Tempo total da requisição por status dos pedidos: {stopwatch.ElapsedMilliseconds} ms" );
                Console.WriteLine( $"Tempo total da requisição por status dos pedidos: {stopwatch.ElapsedMilliseconds} ms" );

                return Ok( response );
            }

            return NotFound( );
        }
    }
}
