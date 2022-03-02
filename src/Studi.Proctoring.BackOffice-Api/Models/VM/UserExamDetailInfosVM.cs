using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Models.VM
{
    public class UserExamDetailInfosVM : UserExamGeneralInfosVM
    {
        public int SessionExamId { get; set; } // SessionExamId
        public bool? IsRqth { get; set; } // IsRqth
        public int ExamTotalDuration { get; set; } // ExamTotalDuration
        public int? ExamActualDuration { get; set; } // ExamActualDuration
        public DateTime? ActualStartTime { get; set; } // ActualStartTime
        public DateTime? ActualEndTime { get; set; } // ActualEndTime
        public decimal TimeZoneShiftToUtc { get; set; } // TimeZoneShiftToUTC
        public int? IdentityDocumentType { get; set; } // IdentityDocumentType
        public int? UserIdentityCheckImage { get; set; } // UserIdentityCheckImage
        public DateTime? HasUserPictureBeenProvided { get; set; } // HasUserPictureBeenProvided
        public int? UserPictureCheckImage { get; set; } // UserPictureCheckImage
        public int? UploadSpeedTest { get; set; } // UploadSpeedTest
        public int? DownloadSpeedTest { get; set; } // DownloadSpeedTest
        public string UserInfrastructure { get; set; } // UserInfrastructure (length: 150)
    }
}
