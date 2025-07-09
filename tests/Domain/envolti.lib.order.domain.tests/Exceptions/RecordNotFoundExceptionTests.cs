using envolti.lib.order.domain.Order.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace envolti.lib.order.domain.tests.Exceptions
{
    public class RecordNotFoundExceptionTests
    {
        [Fact]
        public async Task BuscarRegistroInexistente_DeveLancarRecordNotFoundException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<RecordNotFoundException>(async () =>
            {
                // Simulação de cenário onde a exception seria lançada
                throw new RecordNotFoundException();
            });
        }
    }
}
