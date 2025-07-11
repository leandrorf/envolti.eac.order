﻿namespace envolti.lib.order.application.Mediator.Interfaces
{
    public interface IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle( TRequest request, CancellationToken cancellationToken );
    }
}
