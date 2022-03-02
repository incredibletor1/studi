using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.BackOffice_Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Repositories
{
    public interface IExamSessionRepository
    {
        Task<int> GetCountExamsSessionsFilteredAsync(bool getArchivedExams = false, string searchedString = null);
        Task<IEnumerable<ExamSessionView>> GetExamsSessionsFilteredPaginatedAsync(int PageSize, int PageIndex, bool getArchivedExams = false, string searchedString = null, string orderByPropertyName = "Id", SortDirection orderByDirection = SortDirection.OrderByDescending);
        Task ArchivateExamSessionByIdAsync(int examSessionId, string userArchive);
        Task UnarchivateExamSessionByIdAsync(int examSessionId, string userUnarchive);
        Task CheckPermissionToArchivateExamSessionByIdAsync(int examSessionId);
        Task<ExamSessionDto> GetExamSessionByIdAsync(int examSessionId);
        Task DeleteExamSessionByIdAsync(int examSessionId, string userDelete);
        Task<IEnumerable<ExamSessionDto>> GetMustBeSystemDeletedExamSessionsAsync();
    }
}
