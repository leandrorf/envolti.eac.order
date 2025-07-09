using envolti.lib.order.domain.Order.Entities;
using envolti.lib.order.domain.Order.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace envolti.lib.order.domain.tests.Entities
{
    public class OrderEntityMappingTests
    {
        [Fact]
        public void MapEntityToDto_DeveRetornarDtoCorreto()
        {
            // Arrange
            var produto = new ProductEntity
            {
                Id = "507f1f77bcf86cd799439011",
                ProductIdExternal = 101,
                Name = "Mouse Gamer",
                Price = 150.00m,
                CreatedAt = DateTime.Now
            };

            var order = new OrderEntity
            {
                Id = "507f191e810c19729de860ea",
                OrderIdExternal = 999,
                CreatedAt = new DateTime(2023, 7, 9),
                Status = StatusEnum.Created,
                Products = new List<ProductEntity> { produto },
                TotalPrice = 150.00m,
                ProcessedIn = new DateTime(2023, 7, 10)
            };

            // Act
            var dto = order.MapEntityToDto();

            // Assert
            Assert.Equal(order.Id, dto.Id);
            Assert.Equal(order.OrderIdExternal, dto.OrderIdExternal);
            Assert.Equal(order.TotalPrice, dto.TotalPrice);
            Assert.Equal(order.CreatedAt, dto.CreatedAt);
            Assert.Equal(order.ProcessedIn, dto.ProcessedIn);
            Assert.Equal(order.Status, dto.Status);
            Assert.Single(dto.Products);
            Assert.Equal(produto.Id, dto.Products[0].Id);
            Assert.Equal(produto.Name, dto.Products[0].Name);
        }
    }
}
