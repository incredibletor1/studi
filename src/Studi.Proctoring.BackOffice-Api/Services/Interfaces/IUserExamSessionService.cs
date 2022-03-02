using Studi.Proctoring.BackOffice_Api.Models;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.BackOffice_Api.Models.VM;
using Studi.Proctoring.Database.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Services
{
    public interface IUserExamSessionService
    {
        Task ArchivateUserExamSessionsByExamSessionIdAsync(int examSessionId, string userArchive);
        Task UnarchivateUserExamSessionsByExamSessionIdAsync(int examSessionId, string userUnarchive);
        Task DeleteUserExamSessionsAsync(IEnumerable<UserExamSessionDto> userExamSessionsToDelete, string deleteUserName);
        Task<IEnumerable<UserExamSessionDto>> GetObsoleteUserExamSessionsToDeleteAsync();
        Task<UserExamGeneralInfosPageDto> GetUserExamSessionsFilteredPaginatedByExamSessionIdAsync(int examSessionId, int PageSize, int PageIndex, string searchedString = null, string orderByPropertyName = "Id", SortDirection orderByDirection = SortDirection.OrderByDescending);
        Task<IEnumerable<UserExamSessionDto>> GetUserExamSessionsByExamSessionIdAsync(int examSessionId);
        Task<UserExamSessionDto> GetUserExamSessionByIdAsync(int id);
        Task<ExamStatusEnum> GetExamSessionStatusByExamSessionIdAsync(int id);
        Task<IEnumerable<UserExamForDownloadDto>> GetDownloadInformationsAboutUserExamSessionsOfExamSessionByExamSessionIdAsync(int examSessionId);
    }
}
