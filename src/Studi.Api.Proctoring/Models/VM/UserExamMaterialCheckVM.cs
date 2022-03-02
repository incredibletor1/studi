using Studi.Proctoring.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Api.Proctoring.Models.VM
{
    public class UserExamMaterialCheckVM
    {
        public int UserExamSessionId { get; set; }
        public bool? HasUserIdentityDocBeenProvided { get; set; }
        public IDTypeEnum? IdentityDocumentType { get; set; }
        public bool? HasUserPictureBeenProvided { get; set; }
        public bool? HasUserConnectionBeenTested { get; set; }
        public int? UploadSpeedTest { get; set; }
        public int? DownloadSpeedTest { get; set; }
        public bool? HasMicrophone { get; set; }
        public bool? HasWebcam { get; set; }
        public string UserInfrastructure { get; set; }
    }
}
