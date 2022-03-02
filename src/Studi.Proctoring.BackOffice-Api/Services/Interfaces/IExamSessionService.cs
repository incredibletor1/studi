using Studi.Proctoring.BackOffice_Api.Middlewares.Exceptions;
using Studi.Proctoring.BackOffice_Api.Models;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.Database.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Services
{
    public interface IExamSessionService
    {
        Task<ExamSessionViewPageDto> GetExamsSessionsFilteredPaginatedAsync(int PageSize, int PageIndex, bool getArchivedExams = false, string searchedString = null, string orderByPropertyName = "Id", SortDirection orderByDirection = SortDirection.OrderByDescending);
        Task ArchivateExamSessionByIdAsync(int examSessionId, string userArchive);
        Task UnarchivateExamSessionByIdAsync(int examSessionId, string userUnarchive);
        Task CheckPermissionToArchivateExamSessionByIdAsync(int examSessionId);
        Task<List<int>> DeleteObsoleteExamSessionsAsync(string deleteUserName);
        Task<ExamSessionDto> GetExamSessionByIdAsync(int id);
    }
}
