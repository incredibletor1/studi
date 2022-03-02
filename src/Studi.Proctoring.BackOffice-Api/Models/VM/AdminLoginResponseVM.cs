using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Models.VM
{
    public class AdminLoginResponseVM
    {
        public string AccessToken { get; set; }

        public string Role { get; set; }
    }
}
