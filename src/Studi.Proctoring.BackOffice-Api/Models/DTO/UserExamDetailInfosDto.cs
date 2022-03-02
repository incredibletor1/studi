using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Models.DTO
{
    public class UserExamDetailInfosDto : UserExamGeneralInfosDto
    {
        public int SessionExamId { get; set; } // SessionExamId
        public string SessionExamName { get; set; } // SessionExamName
        public int ExamTotalDuration { get; set; } // ExamTotalDuration
        public int? ExamActualDuration { get; set; } // ExamActualDuration
        public DateTime? ActualStartTime { get; set; } // ActualStartTime
        public DateTime? ActualEndTime { get; set; } // ActualEndTime
        public decimal TimeZoneShiftToUtc { get; set; } // TimeZoneShiftToUTC
        public int? IdentityDocumentType { get; set; } // IdentityDocumentType
        public string? UserIdentityCheckImage { get; set; } // UserIdentityCheckImage
        public bool? HasUserPictureBeenProvided { get; set; } // HasUserPictureBeenProvided
        public string? UserPictureCheckImage { get; set; } // UserPictureCheckImage
        public int? UploadSpeedTest { get; set; } // UploadSpeedTest
        public int? DownloadSpeedTest { get; set; } // DownloadSpeedTest
        public string UserInfrastructure { get; set; } // UserInfrastructure (length: 150)
    }
}
