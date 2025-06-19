using envolti.lib.order.application.Order.Commands;
using envolti.lib.order.application.Order.Responses;
using envolti.lib.order.domain.Order.Dtos;
using envolti.lib.order.domain.Order.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace envolti.api.order.driving.Controllers
{
    [Route( "api/[controller]" )]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _Mediator;
        private readonly ILogger<OrdersController> _Logger;

        public OrdersController( IMediator mediator, ILogger<OrdersController> logger )
        {
            _Mediator = mediator;
            _Logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponse>> Post( [FromBody] OrderRequestDto value )
        {
            Stopwatch stopwatch = Stopwatch.StartNew( );

            var command = new PublishOrderCommand
            {
                Data = value
            };

            var res = await _Mediator.Send( command );

            if ( res.Success )
            {
                stopwatch.Stop( );
                _Logger.LogInformation( $"Tempo total da requisição: {stopwatch.ElapsedMilliseconds} ms" );
                Console.WriteLine( $"Tempo total da requisição: {stopwatch.ElapsedMilliseconds} ms" );

                return Ok( res.Data );
            }
            else if ( res.ErrorCode == ErrorCodesResponseEnum.THE_ORDER_NUMBER_CANNOT_BE_REPEATED )
            {
                _Logger.LogWarning( "The order number cannot be repeated." );
                return Conflict( res );
            }

            _Logger.LogError( "An unexpected error occurred: {Message}", res.Message );
            return BadRequest( res );
        }
    }
}
