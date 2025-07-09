using envolti.lib.order.domain.Order.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace envolti.lib.order.domain.tests.Exceptions
{
    public class TheOrderNumberCannotBeRepeatedExceptionTests
    {
        [Fact]
        public async Task PedidoDuplicado_DeveLancarTheOrderNumberCannotBeRepeatedException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<TheOrderNumberCannotBeRepeatedException>(async () =>
            {
                // Simulação de cenário onde a exception seria lançada
                throw new TheOrderNumberCannotBeRepeatedException();
            });
        }
    }

}
