using Studi.Proctoring.BackOffice_Api.Models;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Repositories
{
    public interface IUserExamSessionRepository
    {
        Task DeleteUserExamSessionByIdAsync(int userExamSessionId, string userDelete);
        Task<IEnumerable<UserExamSessionDto>> GetObsoleteUserExamSessionsToDeleteAsync();
        Task ArchivateUserExamSessionByIdAsync(int userExamSessionId, string userArchive);
        Task UnarchivateUserExamSessionByIdAsync(int userExamSessionId, string userUnarchive);
        Task<IEnumerable<UserExamSessionDto>> GetUserExamSessionsByExamSessionIdAsync(int examSessionId, bool getArchivedUserExams = false);
        Task<int> GetCountUserExamSessionsFilteredAsync(int examSessionId, string searchedString = null);
        Task<IEnumerable<UserExamGeneralInfosDto>> GetUserExamSessionsFilteredPaginatedAsync(int examSessionId, int PageSize, int PageIndex, string searchedString = null, string orderByPropertyName = "Id", SortDirection orderByDirection = SortDirection.OrderByDescending);
        Task<UserExamDetailInfosDto> GetUserExamSessionAllInfosByIdAsync(int id);
        Task<IEnumerable<UserExamSessionDto>> GetUserExamSessionsToFinishWithErrorStatusAsync();
        Task UpdateUserExamSessionAsync(UserExamSessionDto userExamSessionDto, string userUpdate);
    }
}
