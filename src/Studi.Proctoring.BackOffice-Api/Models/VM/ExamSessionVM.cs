using System;
using System.Collections.Generic;

namespace Studi.Proctoring.BackOffice_Api.Models.VM
{
    public class ExamSessionVM
    {
        public int Id { get; set; } // Id (Primary key)
        public string FiliereName { get; set; } // FiliereName (length: 255)
        public string PromotionName { get; set; } // PromotionName (length: 50)
        public string ParcoursName { get; set; } // ParcoursName (length: 255)
        public string EvalBlockName { get; set; } // EvalBlockName (length: 255)
        public string RessourceName { get; set; } // RessourceName (length: 255)
        public int? RessourceVersionId { get; set; } //EvaluationVersionId
        public int ExamInitialDuration { get; set; } // ExamInitialDuration
        public DateTime ScheduledBeginStartTime { get; set; } // ScheduledBeginStartTime
        public DateTime ScheduledEndStartTime { get; set; } // ScheduledEndStartTime
        public int? IntervalProctoringImages { get; set; } // IntervalProctoringImages
        public int UsersCount { get; set; }
        public string ExamStatus { get; set; }
    }

    public class ExamSessionPageVM
    {
        public int PageIndex { get; set; }
        public int TotalItemsCount { get; set; }
        public List<ExamSessionVM> Page { get; set; }
    }
}
