using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class OrderDTO
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; }
        public int Minutes { get; set; }
        public bool Canceled { get; set; }
        public bool Shipped { get; set; }
    }
}
