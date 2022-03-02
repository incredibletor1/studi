using System;

namespace Studi.Proctoring.BackOffice_Api.Models.DTO
{
    public class UserExamSessionDto
    {
        public int Id { get; set; } // Id (Primary key)
        public int UserId { get; set; } // UserId
        public int SessionExamId { get; set; } // SessionExamId
        public bool? IsRqth { get; set; } // IsRqth
        public int StatusId { get; set; } // StatusId
        public int ExamTotalDuration { get; set; } // ExamTotalDuration
        public int? ExamActualDuration { get; set; } // ExamActualDuration
        public DateTime? ScheduledSpecificBeginStartTime { get; set; }
        public DateTime? ScheduledSpecificEndStartTime { get; set; }
        public DateTime? ActualStartTime { get; set; } // ActualStartTime
        public DateTime? ActualEndTime { get; set; } // ActualEndTime
        public decimal TimeZoneShiftToUtc { get; set; } // TimeZoneShiftToUTC
        public bool? HasUserIdentityDocBeenProvided { get; set; } // HasUserIdentityDocBeenProvided
        public int? IdentityDocumentType { get; set; } // IdentityDocumentType
        public bool? HasUserPictureBeenProvided { get; set; } // HasUserPictureBeenProvided
        public bool? HasUserConnectionBeenTested { get; set; } // HasUserConnectionBeenTested
        public int? UploadSpeedTest { get; set; } // UploadSpeedTest
        public int? DownloadSpeedTest { get; set; } // DownloadSpeedTest
        public int? ConnectionQuality { get; set; } // ConnectionQuality
        public bool? HasMicrophone { get; set; } // HasMicrophone
        public bool? HasWebcam { get; set; } // HasWebcam
        public string UserInfrastructure { get; set; } // UserInfrastructure (length: 150)

    }
}
