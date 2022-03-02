using System;
using System.Collections.Generic;

using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.BackOffice_Api.Helpers;
using System.Threading.Tasks;
using Studi.Proctoring.BackOffice_Api.Repositories;
using Studi.Proctoring.BackOffice_Api.Models;
using Studi.Proctoring.Database.Context;

namespace Studi.Proctoring.BackOffice_Api.Services
{
    public class ExamSessionService : IExamSessionService
    {
        /// <summary>
        /// Defines the services.
        /// </summary>
        private readonly IExamSessionRepository examSessionRepository;

        public ExamSessionService(IServiceProvider serviceProvider)
        {
            examSessionRepository = serviceProvider.ExamSessionRepository();
        }

        public async Task<ExamSessionViewPageDto> GetExamsSessionsFilteredPaginatedAsync(int PageSize, int PageIndex, bool getArchivedExams = false, string searchedString = null, string orderByPropertyName = "Id", SortDirection orderByDirection = SortDirection.OrderByDescending)
        {
            var examsSessionsCount = await examSessionRepository.GetCountExamsSessionsFilteredAsync(
                getArchivedExams, searchedString);
            var examsSessionsPage = await examSessionRepository.GetExamsSessionsFilteredPaginatedAsync(
                    PageSize, PageIndex, getArchivedExams, searchedString, orderByPropertyName, orderByDirection);
            return new ExamSessionViewPageDto
            {
                PageIndex = PageIndex,
                TotalItemsCount = examsSessionsCount,
                Page = examsSessionsPage
            };
        }

        /// <summary>
        /// CheckPermissionToArchivateExamSessionById
        /// </summary>
        /// <param name="examSessionId"></param>
        public async Task CheckPermissionToArchivateExamSessionByIdAsync(int examSessionId)
        {
            await examSessionRepository.CheckPermissionToArchivateExamSessionByIdAsync(examSessionId);
        }

        /// <summary>
        /// ArchivateExamSessionById
        /// </summary>
        /// <param name="examSessionId"></param>
        public async Task ArchivateExamSessionByIdAsync(int examSessionId, string userArchive)
        {
            await examSessionRepository.ArchivateExamSessionByIdAsync(examSessionId, userArchive);
        }

        /// <summary>
        /// UnarchivateExamSessionById
        /// </summary>
        /// <param name="examSessionId"></param>
        public async Task UnarchivateExamSessionByIdAsync(int examSessionId, string userUnarchive)
        {
            await examSessionRepository.UnarchivateExamSessionByIdAsync(examSessionId, userUnarchive);
        }

        /// <summary>
        /// Get a examSession by its id
        /// </summary>
        /// <param name="id">the examSession id</param>
        /// <returns>returns the corresponding examSession</returns>
        public async Task<ExamSessionDto> GetExamSessionByIdAsync(int id)
        {
            return await examSessionRepository.GetExamSessionByIdAsync(id);
        }

        /// <summary>
        /// DeleteAllExamSessionsOlderThan6Months
        /// </summary>
        public async Task<List<int>> DeleteObsoleteExamSessionsAsync(string deleteUserName)
        {
            var examSessions = await examSessionRepository.GetMustBeSystemDeletedExamSessionsAsync();
            var deletedExamSessionsIds = new List<int>();

            foreach (var examSession in examSessions)
            {
                await examSessionRepository.DeleteExamSessionByIdAsync(examSession.Id, deleteUserName);
                deletedExamSessionsIds.Add(examSession.Id);
            }

            return deletedExamSessionsIds;
        }
    }
}
