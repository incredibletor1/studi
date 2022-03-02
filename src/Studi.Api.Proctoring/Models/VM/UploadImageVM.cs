using Microsoft.AspNetCore.Http;

namespace Studi.Api.Proctoring.Models.VM
{
    public class UploadImageVM
    {
        public IFormFile ImageFile { get; set; }
        public string EvaluationCode { get; set; }
        public string FiliereCode { get; set; }
        public string PromotionCode { get; set; }
    }
}
