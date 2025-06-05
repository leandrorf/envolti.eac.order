using envolti.lib.order.application.Order.Commands;
using envolti.lib.order.application.Order.Responses;
using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace envolti.api.order.driving.Controllers
{
    [Route( "api/[controller]" )]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _Mediator;

        public OrdersController( IMediator mediator )
        {
            _Mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponse>> Post( [FromBody] OrderRequestDto value )
        {
            var command = new PublishOrderCommand
            {
                Data = value
            };

            var res = await _Mediator.Send( command );

            if ( res.Success )
            {
                return Ok( res.Data );
            }
            else if ( res.ErrorCode == ErrorCodesResponseEnum.THE_ORDER_NUMBER_CANNOT_BE_REPEATED )
            {
                return Conflict( res );
            }

            return BadRequest( res );
        }
    }
}
