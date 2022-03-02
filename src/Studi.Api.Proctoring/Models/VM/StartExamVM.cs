using Microsoft.AspNetCore.Http;
using System;

namespace Studi.Api.Proctoring.Models.VM
{
    public class StartExamVM
    {
        public string FiliereCode { get; set; }
        public string PromotionCode { get; set; }
        public string EvaluationCode { get; set; } // Represents LMS RessourceCode
        public int EvaluationVersionId { get; set; } // Represents LMS RessourceVersionId
        public string Email { get; set; }
        public IFormFile Photo { get; set; }
        public IFormFile PhotoId { get; set; }
        public int? PhotoIdType { get; set; }
    }
}
