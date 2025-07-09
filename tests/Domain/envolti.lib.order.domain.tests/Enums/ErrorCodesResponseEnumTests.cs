using envolti.lib.order.domain.Order.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace envolti.lib.order.domain.tests.Enums
{
    public class ErrorCodesResponseEnumTests
    {
        [Fact]
        public void Enum_DeveConterValoresEsperados()
        {
            Assert.Equal(1, (int)ErrorCodesResponseEnum.THE_ORDER_NUMBER_CANNOT_BE_REPEATED);
            Assert.Equal(97, (int)ErrorCodesResponseEnum.RECORD_NOT_FOUND);
            Assert.Equal(98, (int)ErrorCodesResponseEnum.NO_RECORDS_FOUND);
            Assert.Equal(99, (int)ErrorCodesResponseEnum.UNIDENTIFIED_ERROR);
        }

        [Fact]
        public void Enum_DeveConterTodosOsValoresDefinidos()
        {
            // Arrange: lista esperada
            var esperados = new[]
            {
                ErrorCodesResponseEnum.THE_ORDER_NUMBER_CANNOT_BE_REPEATED,
                ErrorCodesResponseEnum.RECORD_NOT_FOUND,
                ErrorCodesResponseEnum.NO_RECORDS_FOUND,
                ErrorCodesResponseEnum.UNIDENTIFIED_ERROR
            };

            // Act: todos os valores reais
            var reais = Enum.GetValues(typeof(ErrorCodesResponseEnum)).Cast<ErrorCodesResponseEnum>();

            // Assert: verifica se os dois conjuntos são iguais
            Assert.Equal(esperados.OrderBy(e => e), reais.OrderBy(r => r));
        }
    }
}
