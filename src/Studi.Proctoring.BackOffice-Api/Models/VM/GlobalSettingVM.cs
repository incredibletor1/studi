using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Models.VM
{
    public class GlobalSettingVM
    {
        public int Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 50)
        public string Name { get; set; } // Name (length: 255)
        public int DataTypeId { get; set; } // DataTypeId
        public string Unit { get; set; } // Unit (length: 50)
        public string Value { get; set; } // Value (length: 128)
        public string OversizedValue { get; set; } // OversizedValue
    }

    public class GlobalSettingsVM : List<GlobalSettingVM>
    {
    }
}
