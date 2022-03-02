using Studi.Proctoring.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Models.DTO
{
    public class UserExamGeneralInfosDto
    {
        public int Id { get; set; } // Id (Primary key)
        public int UserId { get; set; } // UserId
        public string UserFirstnamePlusLastname { get; set; } // Candidat
        public string UserEmail { get; set; } // User's Email
        public string Status { get; set; } // get from StatusId
        public bool? HasUserIdentityDocBeenProvided { get; set; } // from HasUserIdentityDocBeenProvided
        public bool? HasUserConnectionBeenTested { get; set; } // HasUserConnectionBeenTested
        public int? ConnectionQuality { get; set; } // ConnectionQuality
        public bool? HasMicrophone { get; set; } // HasMicrophone
        public bool? HasWebcam { get; set; } // HasWebcam
    }

    public class UserExamGeneralInfosPageDto
    {
        public string ExamSessionName { get; set; }
        public int PageIndex { get; set; }
        public int TotalItemsCount { get; set; }
        public IEnumerable<UserExamGeneralInfosDto> UserExamsPage { get; set; }
    }
}
