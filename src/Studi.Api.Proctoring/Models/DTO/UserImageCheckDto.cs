using System;

namespace Studi.Api.Proctoring.Models.DTO
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
