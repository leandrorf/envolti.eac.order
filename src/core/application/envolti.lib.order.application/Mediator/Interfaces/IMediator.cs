namespace envolti.lib.order.application.Mediator.Interfaces
{
    public interface IMediator
    {
        Task<TResponse> Send<TResponse>( IRequest<TResponse> request, CancellationToken cancellationToken = default );
    }
}
