using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Models.VM
{
    public class AdminBasicLoginVM
    {
        public string Login { get; set; }       // Login (length: 50)
        public string Password { get; set; }    // Password (length: 255)
    }
}
