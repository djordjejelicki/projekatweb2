using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class GoogleSigninToken
    {
        public string GoogleAccessToken { get; set; }
        public string Role { get; set; }
    }
}
