using Studi.Proctoring.BackOffice_Api.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Repositories
{
    public interface IProctoringRepository
    {
        Task DeleteFromDBUserCheckPicturesAsync(int userExamSessionId, string userDelete);
        Task DeleteFromDBProctoringPicturesAsync(int userExamSessionId, string userDelete);
        Task<IEnumerable<ProctoringImageForUserExamDto>> GetProctoringImagesPaginatedByUserExam(int userExamSessionId, int pageSize, int pageIndex);
        Task<int> GetCountProctoringImagesPaginatedByUserExamAsync(int userExamSessionId);
    }
}
