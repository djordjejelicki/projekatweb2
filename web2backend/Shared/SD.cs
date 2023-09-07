using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class SD
    {
        [Flags]
        public enum Roles : byte 
        {
            Buyer = 1,
            Seller = 2,
            Admin = 3
        }
    }
}
