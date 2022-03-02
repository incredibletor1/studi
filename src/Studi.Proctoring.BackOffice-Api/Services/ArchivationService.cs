using Studi.Proctoring.BackOffice_Api.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Services
{
    public class ArchivationService : IArchivationService
    {
        private readonly IUserExamSessionService userExamSessionService;
        private readonly IExamSessionService examSessionService;

        public ArchivationService(IServiceProvider serviceProvider)
        {
            userExamSessionService = serviceProvider.UserExamSessionService();
            examSessionService = serviceProvider.ExamSessionService();
        }

        /// <summary>
        /// ArchivateExamSessionById
        /// </summary>
        /// <param name="examSessionId"></param>
        public async Task ArchivateExamSessionByIdAsync(int examSessionId, string userArchive)
        {
            await examSessionService.CheckPermissionToArchivateExamSessionByIdAsync(examSessionId);
            await userExamSessionService.ArchivateUserExamSessionsByExamSessionIdAsync(examSessionId, userArchive);
            await examSessionService.ArchivateExamSessionByIdAsync(examSessionId, userArchive);
        }

        /// <summary>
        /// UnarchivateExamSessionById
        /// </summary>
        /// <param name="examSessionId"></param>
        public async Task UnarchivateExamSessionByIdAsync(int examSessionId, string userUnarchive)
        {
            await userExamSessionService.UnarchivateUserExamSessionsByExamSessionIdAsync(examSessionId, userUnarchive);
            await examSessionService.UnarchivateExamSessionByIdAsync(examSessionId, userUnarchive);
        }

        /// <summary>
        /// UnarchivateExamSessionListAsync
        /// </summary>
        /// <param name="examSessionIds"></param>
        public async Task UnarchivateExamSessionListAsync(List<int> examSessionIds, string userUnarchive)
        {
            foreach (var examSessionId in examSessionIds)
            {
                await userExamSessionService.UnarchivateUserExamSessionsByExamSessionIdAsync(examSessionId, userUnarchive);
                await examSessionService.UnarchivateExamSessionByIdAsync(examSessionId, userUnarchive);
            }
        }
    }
}
