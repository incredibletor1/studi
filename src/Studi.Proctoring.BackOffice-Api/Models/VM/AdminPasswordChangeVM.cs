using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Models.VM
{
    public class AdminPasswordChangeVM : AdminBasicLoginVM
    {
        public string NewPassword { get; set; }
    }
}
