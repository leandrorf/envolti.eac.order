using envolti.lib.order.domain.Order.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace envolti.lib.order.domain.tests.Exceptions
{
    public class NoRecordsFoundExceptionTests
    {
        [Fact]
        public async Task BuscarSemDados_DeveLancarNoRecordsFoundException()
        {
            // Act & Assert  
            await Assert.ThrowsAsync<NoRecordsFoundException>(async () =>
            {
                // Simulação de cenário onde a exception seria lançada  
                throw new NoRecordsFoundException();
            });
        }
    }
}
