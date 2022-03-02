using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Models.DTO
{
    public class ProctoringImageForUserExamDto
    {
        public DateTime ImageTimeStamp { get; set; } // ImageTimeStamp
        public byte[] ImageByte { get; set; } // Image byte[]
    }

    public class ProctoringImageForUserExamPageDto
    {
        public int PageIndex { get; set; }
        public int TotalItemsCount { get; set; }
        public IEnumerable<ProctoringImageForUserExamDto> ProctoringImagesPage { get; set; }
    }
}
