using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Models.DTO
{
    public class UserImageCheckDto
    {
        public int Id { get; set; } // Id (Primary key)
        public int UserExamSessionId { get; set; } // UserExamSessionId
        public int ImageTypeId { get; set; } // ImageTypeId
        public DateTime ImageTimeStamp { get; set; } // ImageTimeStamp
        public int ContainerTypeId { get; set; } // ContainerTypeId
        public string ContainerImageId { get; set; } // ContainerImageId (length: 300)
    }
}
