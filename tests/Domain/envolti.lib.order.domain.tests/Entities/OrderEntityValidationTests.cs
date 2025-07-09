using envolti.lib.order.domain.Order.Entities;
using envolti.lib.order.domain.Order.Enums;
using envolti.lib.order.domain.Order.Exceptions;
using envolti.lib.order.domain.Order.Ports;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace envolti.lib.order.domain.tests.Entities
{
    public class OrderEntityValidationTests
    {
        [Fact]
        public async Task Save_PedidoExistente_DeveLancarExcecao()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            var order = new OrderEntity
            {
                OrderIdExternal = 123,
                Products = [],
                Status = StatusEnum.Created
            };

            // Simula que o pedido já existe
            mockRepo.Setup(r => r.OrderExistsAsync(order.OrderIdExternal))
                    .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<TheOrderNumberCannotBeRepeatedException>(() =>
                order.Save(mockRepo.Object));

            // Verifica que não chamou CreateOrderAsync
            mockRepo.Verify(r => r.CreateOrderAsync(It.IsAny<OrderEntity>()), Times.Never);
        }
    }
}
