﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace envolti.lib.order.application.Mediator.Interfaces
{
    public interface IMediator
    {
        Task<TResponse> Send<TResponse>( IRequest<TResponse> request, CancellationToken cancellationToken = default );
    }
}
