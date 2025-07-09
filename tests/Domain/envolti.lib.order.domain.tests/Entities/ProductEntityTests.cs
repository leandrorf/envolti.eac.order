using envolti.lib.order.domain.Order.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace envolti.lib.order.domain.tests.Entities;

public class ProductEntityTests
{
    [Fact]
    public void CriarProduto_DeveInicializarCorretamente()
    {
        // Arrange
        var idExterno = 123;
        var nome = "Teclado Mecânico";
        var preco = 299.90m;
        var dataCriacao = DateTime.Now;

        // Act
        var produto = new ProductEntity
        {
            ProductIdExternal = idExterno,
            Name = nome,
            Price = preco,
            CreatedAt = dataCriacao
        };

        // Assert
        Assert.Equal(idExterno, produto.ProductIdExternal);
        Assert.Equal(nome, produto.Name);
        Assert.Equal(preco, produto.Price);
        Assert.Equal(dataCriacao, produto.CreatedAt);
        Assert.Null(produto.Id); // Esperado já que não foi definido
    }
}
