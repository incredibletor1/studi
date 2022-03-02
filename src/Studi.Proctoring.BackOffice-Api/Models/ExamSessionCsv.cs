using Studi.Proctoring.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Models
{
    public class ExamSessionCsv
    {
        public string RessourceName { get; set; } // RessourceName (length: 255)
        public string FiliereName { get; set; } // FiliereName (length: 255)
        public string PromotionName { get; set; } // PromotionName (length: 50)
        public string ParcoursName { get; set; } // ParcoursName (length: 255)
        public string EvalBlockName { get; set; } // EvalBlockName (length: 255)
        public DateTime ScheduledBeginStartTime { get; set; } // ScheduledBeginStartTime
        public DateTime ScheduledEndStartTime { get; set; } // ScheduledEndStartTime
        public int UsersCount { get; set; }
        public ExamStatusEnum ExamStatus { get; set; }
    }
}
