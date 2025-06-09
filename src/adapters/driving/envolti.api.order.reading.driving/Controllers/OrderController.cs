using envolti.lib.order.application.Order.Queries;
using envolti.lib.order.application.Order.Responses;
using envolti.lib.order.domain.Order.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace envolti.api.order.reading.driving.Controllers
{
    [Route( "api/[controller]" )]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrderController( IMediator mediator )
        {
            _mediator = mediator;
        }

        [HttpGet( "{OrderIdExternal}" )]
        public async Task<ActionResult<OrderResponse>> Get( int OrderIdExternal )
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
                Console.WriteLine( $"Tempo total da requisição do pedido por Id: {stopwatch.ElapsedMilliseconds} ms" );

                return Ok( response.Data );
            }

            return NotFound( response.Message );
        }

        [HttpGet( "GetAll" )]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetAll( )
        {
            Stopwatch stopwatch = Stopwatch.StartNew( );

            var response = await _mediator.Send( new GetAllOrdersQuery( ) );

            if ( response.Any( ) )
            {
                stopwatch.Stop( );
                Console.WriteLine( $"Tempo total da requisição por todos os pedidos: {stopwatch.ElapsedMilliseconds} ms" );

                return Ok( response );
            }

            return NotFound( );
        }

        [HttpGet( "GetByStatus" )]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetByStatus( StatusEnum status )
        {
            Stopwatch stopwatch = Stopwatch.StartNew( );

            var query = new GetOrdersByStatusQuery
            {
                Status = status
            };

            var response = await _mediator.Send( query );

            if ( response.Any( ) )
            {
                stopwatch.Stop( );
                Console.WriteLine( $"Tempo total da requisição por status dos pedidos: {stopwatch.ElapsedMilliseconds} ms" );

                return Ok( response );
            }

            return NotFound( );
        }
    }
}
