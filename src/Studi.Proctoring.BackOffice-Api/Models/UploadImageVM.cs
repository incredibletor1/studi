using Microsoft.AspNetCore.Http;

namespace Studi.Proctoring.BackOffice_Api.Models.VM
{
    public class UploadImageVM
    {
        public IFormFile ImageFile { get; set; }
        public string EvaluationCode { get; set; }
        public string FiliereCode { get; set; }
        public string PromotionCode { get; set; }
    }
}
