using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class GoogleTokenInfo
    {
        public string Sub { get; set; } // User ID
        public string Name { get; set; } // User's full name
        public string GivenName { get; set; } // User's first name
        public string FamilyName { get; set; } // User's last name
        public string Email { get; set; } // User's email address
        public bool EmailVerified { get; set; } // Indicates if email is verified
        public string Picture { get; set; } // URL of user's profile picture
        public string Locale { get; set; } // User's preferred locale/language
    }
}
