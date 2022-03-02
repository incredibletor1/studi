using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Exceptions
{
    public class GlobalSettingWrongValueTypeException : Exception
    {
        public GlobalSettingWrongValueTypeException(string message) : base (message)
        {
        }
    }
}
