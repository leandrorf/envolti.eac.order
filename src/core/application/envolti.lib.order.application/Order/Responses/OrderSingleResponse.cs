﻿using envolti.lib.order.domain.Order.Dtos;

namespace envolti.lib.order.application.Order.Responses
{
    public class OrderSingleResponse : Response
    {
        public OrderResponseDto? Data { get; set; }
    }
}
