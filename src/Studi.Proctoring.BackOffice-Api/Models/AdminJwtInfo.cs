using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Models
{
    public class AdminJwtInfo
    {
        public string issuer { get; set; }
        public int lifetime { get; set; }   // In minutes
        public string keyString { get; set; }
    }
}
