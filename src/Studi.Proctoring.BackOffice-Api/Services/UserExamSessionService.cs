using Studi.Proctoring.BackOffice_Api.Helpers;
using Studi.Proctoring.BackOffice_Api.Models;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.BackOffice_Api.Repositories;
using Studi.Proctoring.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Services
{
    public class UserExamSessionService : IUserExamSessionService
    {
        /// <summary>
        /// Defines the services.
        /// </summary>
        private readonly IUserService userService;
        private readonly IExamSessionService examSessionService;
        private readonly IUserExamSessionRepository userExamSessionRepository;

        public UserExamSessionService(IServiceProvider serviceProvider)
        {
            userService = serviceProvider.UserService();
            examSessionService = serviceProvider.ExamSessionService();
            userExamSessionRepository = serviceProvider.UserExamSessionRepository();
        }

        /// <summary>
        /// ArchivateUserExamSessionsByExamSessionId
        /// </summary>
        /// <param name="examSessionId">the exam session id</param>
        public async Task ArchivateUserExamSessionsByExamSessionIdAsync(int examSessionId, string userArchive)
        {
            var userExamSessions = await userExamSessionRepository.GetUserExamSessionsByExamSessionIdAsync(examSessionId, false);

            foreach (var userExamSession in userExamSessions)
            {
                await userExamSessionRepository.ArchivateUserExamSessionByIdAsync(userExamSession.Id, userArchive);
            }
        }

        /// <summary>
        /// UnarchivateUserExamSessionsByExamSessionId
        /// </summary>
        /// <param name="examSessionId">the exam session id</param>
        public async Task UnarchivateUserExamSessionsByExamSessionIdAsync(int examSessionId, string userUnarchive)
        {
            var userExamSessions = await userExamSessionRepository.GetUserExamSessionsByExamSessionIdAsync(examSessionId, true);

            foreach (var userExamSession in userExamSessions)
            {
                await userExamSessionRepository.UnarchivateUserExamSessionByIdAsync(userExamSession.Id, userUnarchive);
            }
        }

        /// <summary>
        /// SystemDeleteUserExamSessionsAsync
        /// </summary>
        public async Task DeleteUserExamSessionsAsync(IEnumerable<UserExamSessionDto> userExamSessionsToDelete, string deleteUserName)
        {
            foreach (var userExamSession in userExamSessionsToDelete)
            {
                await userExamSessionRepository.DeleteUserExamSessionByIdAsync(userExamSession.Id, deleteUserName);
            }
        }

        public async Task<IEnumerable<UserExamSessionDto>> GetObsoleteUserExamSessionsToDeleteAsync()
        {
            return await userExamSessionRepository.GetObsoleteUserExamSessionsToDeleteAsync();
        }

        public async Task<UserExamGeneralInfosPageDto> GetUserExamSessionsFilteredPaginatedByExamSessionIdAsync(int examSessionId, int PageSize, int PageIndex, string searchedString = null, string orderByPropertyName = "Id", SortDirection orderByDirection = SortDirection.OrderByDescending)
        {
            var userExamSessionsCount = await userExamSessionRepository.GetCountUserExamSessionsFilteredAsync(examSessionId, searchedString);
            var userExamSessionsPage = await userExamSessionRepository.GetUserExamSessionsFilteredPaginatedAsync(examSessionId, PageSize, PageIndex, searchedString, orderByPropertyName, orderByDirection);
            var examSession = await examSessionService.GetExamSessionByIdAsync(examSessionId);

            return new UserExamGeneralInfosPageDto
            {
                ExamSessionName = examSession.RessourceName,
                PageIndex = PageIndex,
                TotalItemsCount = userExamSessionsCount,
                UserExamsPage = userExamSessionsPage
            };
        }

        public async Task<UserExamDetailInfosDto> GetUserExamDetailsByUserExamSessionIdAsync(int userExamSessionId)
        {
            var userExamSession = await userExamSessionRepository.GetUserExamSessionAllInfosByIdAsync(userExamSessionId);

            return userExamSession;
        }

        /// <summary>
        /// GetUserExamSessionsToFinishWithErrorStatus
        /// </summary>
        /// <returns>corresponding userExamSessions</returns>
        public async Task<IEnumerable<UserExamSessionDto>> GetUserExamSessionsToFinishWithErrorStatusAsync()
        {
            return await userExamSessionRepository.GetUserExamSessionsToFinishWithErrorStatusAsync();
        }

        /// <summary>
        /// UpdateUserExamsStatusToFinishWithError
        /// </summary>
        /// <param name="userExamSessionsToFinishWithError">UserExamSessions to be finishedWithError</param>
        /// <param name="finishWithErrorUserName">User who finish userExamSessions with error</param>
        /// <returns></returns>
        public async Task<List<int>> UpdateUserExamsStatusToFinishWithErrorAsync(IEnumerable<UserExamSessionDto> userExamSessionsToFinishWithError, string finishWithErrorUserName)
        {
            foreach (var userExamSession in userExamSessionsToFinishWithError)
            {
                userExamSession.StatusId = (int)ExamStatusEnum.FinishedWithError;
                await userExamSessionRepository.UpdateUserExamSessionAsync(userExamSession, finishWithErrorUserName);
            }

            return userExamSessionsToFinishWithError.Select(ues => ues.Id).ToList();
        }
    }
}
