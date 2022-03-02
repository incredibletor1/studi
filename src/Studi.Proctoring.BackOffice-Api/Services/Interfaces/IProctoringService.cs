using Studi.Proctoring.BackOffice_Api.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Services
{
    public interface IProctoringService
    {
        Task<ProctoringImageForUserExamPageDto> GetProctoringImagesPaginatedByUserExam(int userExamSessionId, int pageSize, int pageIndex);
    }
}
