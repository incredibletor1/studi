using System;
using System.Collections.Generic;
using Studi.Proctoring.Database.Context;

namespace Studi.Proctoring.BackOffice_Api.Models.DTO
{
    public class ExamSessionDto
    {
        public int Id { get; set; } // Id (Primary key)
        public string FiliereCode { get; set; } // FiliereCode (length: 20)
        public string FiliereName { get; set; } // FiliereName (length: 255)
        public string PromotionCode { get; set; } // PromotionCode (length: 200)
        public string PromotionName { get; set; } // PromotionName (length: 50)
        public string ParcoursCode { get; set; } // ParcoursCode (length: 150)
        public string ParcoursName { get; set; } // ParcoursName (length: 255)
        public string EvalBlockCode { get; set; } // EvalBlockCode (length: 150)
        public string EvalBlockName { get; set; } // EvalBlockName (length: 255)
        public string RessourceCode { get; set; } // RessourceCode (length: 150)
        public string RessourceName { get; set; } // RessourceName (length: 255)
        public int? RessourceVersionId { get; set; } //EvaluationVersionId
        public int ExamInitialDuration { get; set; } // ExamInitialDuration
        public DateTime ScheduledBeginStartTime { get; set; } // ScheduledBeginStartTime
        public DateTime ScheduledEndStartTime { get; set; } // ScheduledEndStartTime
        public int? IntervalProctoringImages { get; set; } // IntervalProctoringImages

    }

    public class ExamSessionView : ExamSessionDto
    {
        public int UsersCount { get; set; }
        public ExamStatusEnum ExamStatus { get; set; }
    }

    public class ExamSessionViewPageDto
    {
        public int PageIndex { get; set; }
        public int TotalItemsCount { get; set; }
        public IEnumerable<ExamSessionView> Page { get; set; }
    }
}
