using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class OrderItemDTO
    {
        public long ItemId { get; set; }
        public long OrderId { get; set; }
        public ItemDTO Item { get; set; }
        public int Amount { get; set; }
        public bool IsSent { get; set; }
        public long SellerId { get; set; }
    }
}
