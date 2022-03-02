using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Models.VM
{
    public class ProctoringImageForUserExamVM
    {
        public DateTime ImageTimeStamp { get; set; } // ImageTimeStamp
        public byte[] ImageByte { get; set; } // Image byte[]
    }

    public class ProctoringImageForUserExamPageVM
    {
        public int PageIndex { get; set; }
        public int TotalItemsCount { get; set; }
        public List<ProctoringImageForUserExamVM> ProctoringImagesPage { get; set; }
    }
}
