using System;
using System.Collections.Generic;

namespace Studi.Proctoring.BackOffice_Api.Models.VM
{
    public class UsersExamsBySessionVM : List<UserExamGeneralInfosVM>
    {
        public int SessionExamId { get; set; } // SessionExamId
    }    
}
