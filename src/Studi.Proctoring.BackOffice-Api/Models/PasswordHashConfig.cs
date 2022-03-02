using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Models
{
    public class PasswordHashConfig
    {
        public int SaltSize { get; set; }   // Maximum salt size is 42, limited by database table column type
        public int HashSize { get; set; }   // Maximum password hash size is 186, limited by database table column type
        public int Iterations { get; set; } // >= 1000 recommended
    }
}
