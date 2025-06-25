using envolti.lib.order.application.Mediator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace envolti.lib.order.application.Mediator
{
    public class MediatorLR : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public MediatorLR( IServiceProvider serviceProvider )
        {
            _serviceProvider = serviceProvider;
        }

        public Task<TResponse> Send<TResponse>( IRequest<TResponse> request, CancellationToken cancellationToken = default )
        {
            var handlerType = typeof( IRequestHandler<,> ).MakeGenericType( request.GetType( ), typeof( TResponse ) );
            var handler = _serviceProvider.GetService( handlerType );

            if ( handler == null )
            {
                throw new InvalidOperationException( $"Handler for '{handlerType}' not found." );
            }

            return ( ( dynamic )handler ).Handle( ( dynamic )request, cancellationToken );
        }
    }
}
