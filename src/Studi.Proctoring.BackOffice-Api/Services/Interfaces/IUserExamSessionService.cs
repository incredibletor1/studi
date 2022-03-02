using Studi.Proctoring.BackOffice_Api.Models;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
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
        Task<UserExamDetailInfosDto> GetUserExamDetailsByUserExamSessionIdAsync(int userExamSessionId);
        Task<IEnumerable<UserExamSessionDto>> GetUserExamSessionsToFinishWithErrorStatusAsync();
        Task<List<int>> UpdateUserExamsStatusToFinishWithErrorAsync(IEnumerable<UserExamSessionDto> userExamSessionsToFinishWithError, string finishWithErrorUserName);
    }
}
