using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Order
    {
        public long Id { get; set; }
        public User User { get; set; }
        public long UserId { get; set; }
        public List<OrderItem> OrderItems { get; set;}
    }
}
