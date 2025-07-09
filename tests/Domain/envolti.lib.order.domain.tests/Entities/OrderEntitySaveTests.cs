using envolti.lib.order.domain.Order.Entities;
using envolti.lib.order.domain.Order.Enums;
using envolti.lib.order.domain.Order.Ports;
using Moq;

namespace envolti.lib.order.domain.tests.Entities
{
    public class OrderEntitySaveTests
    {
        [Fact]
        public async Task Save_NovoPedido_DeveValidarProcessarESalvar()
        {
            // Arrange
            var mockRepo = new Mock<IOrderRepository>();
            var order = new OrderEntity
            {
                OrderIdExternal = 123,
                Products = new List<ProductEntity>
            {
                new ProductEntity
                {
                    ProductIdExternal = 1,
                    Name = "Fone de Ouvido",
                    Price = 200.00m
                }
            },
                Status = StatusEnum.Created,
                CreatedAt = DateTime.Now
            };

            // Simula que pedido ainda não existe
            mockRepo.Setup(r => r.OrderExistsAsync(order.OrderIdExternal))
                    .ReturnsAsync(false);

            // Simula retorno do repositório com Id gerado
            var orderComId = new OrderEntity { Id = "507f191e810c19729de860ea" };
            mockRepo.Setup(r => r.CreateOrderAsync(It.IsAny<OrderEntity>()))
                    .ReturnsAsync(orderComId);

            // Act
            await order.Save(mockRepo.Object);

            // Assert
            Assert.Equal(orderComId.Id, order.Id);
            Assert.Equal(StatusEnum.Processed, order.Status);
            Assert.True(order.TotalPrice > 0);
            Assert.NotNull(order.ProcessedIn);
            Assert.All(order.Products, p => Assert.NotNull(p.Id));
            Assert.All(order.Products, p => Assert.True(p.CreatedAt <= DateTime.Now));

            // Verifica se os métodos do repositório foram chamados corretamente
            mockRepo.Verify(r => r.OrderExistsAsync(order.OrderIdExternal), Times.Once);
            mockRepo.Verify(r => r.CreateOrderAsync(order), Times.Once);
        }

    }
}