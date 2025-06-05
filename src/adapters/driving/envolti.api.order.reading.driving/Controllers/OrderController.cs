using envolti.lib.order.application.Order.Queries;
using envolti.lib.order.application.Order.Responses;
using envolti.lib.order.domain.Order.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
            var query = new GetOrderByIdQuery
            {
                OrderIdExternal = OrderIdExternal
            };

            var response = await _mediator.Send( query );

            if ( response.Success )
            {
                return Ok( response.Data );
            }

            return NotFound( response.Message );
        }

        [HttpGet( "GetAll" )]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetAll( )
        {
            var response = await _mediator.Send( new GetAllOrdersQuery( ) );

            if ( response.Any( ) )
            {
                return Ok( response );
            }

            return NotFound( );
        }

        [HttpGet( "GetByStatus" )]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetByStatus( StatusEnum status )
        {
            var query = new GetOrdersByStatusQuery
            {
                Status = status
            };

            var response = await _mediator.Send( query );

            if ( response.Any( ) )
            {
                return Ok( response );
            }

            return NotFound( );
        }
    }
}
