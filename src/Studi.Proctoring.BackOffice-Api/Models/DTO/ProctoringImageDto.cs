using System;

namespace Studi.Proctoring.BackOffice_Api.Models.DTO
{
    public class ProctoringImageDto
    {
        public int Id { get; set; } // Id (Primary key)
        public int UserExamSessionId { get; set; } // UserExamSessionId
        public DateTime ImageTimeStamp { get; set; } // ImageTimeStamp
        public int Order { get; set; } // Order
        public int ContainerTypeId { get; set; } // ContainerTypeId
        public string ContainerImageId { get; set; } // ContainerImageId (length: 300)
    }
}
