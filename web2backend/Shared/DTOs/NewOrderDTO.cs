using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class NewOrderDTO
    {
        public long UserId { get; set; }
        public string Comment { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public List<NewOrderItemDTO> Items { get; set; }
    }
}
