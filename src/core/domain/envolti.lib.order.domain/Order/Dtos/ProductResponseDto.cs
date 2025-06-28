using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace envolti.lib.order.domain.Order.Dtos
{
    public class ProductResponseDto
    {
        public string? Id { get; set; }
        public required int ProductIdExternal { get; set; }
        public required string Name { get; set; }
        public required decimal Price { get; set; }
    }
}
