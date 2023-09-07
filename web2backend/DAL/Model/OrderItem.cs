using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class OrderItem
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public Order Order { get; set; }
        public long OrderId { get; set; }
        public bool IsSent { get; set; }
        public long SellerId { get; set; }
        public long Amount { get; set; }
    }
}
