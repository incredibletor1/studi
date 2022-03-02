using System;
using System.Collections.Generic;

using Studi.Api.Proctoring.Models.DTO;
using Studi.Api.Proctoring.Helpers;
using System.Threading.Tasks;
using Studi.Api.Proctoring.Repositories;
using Studi.Api.Proctoring.Models;

namespace Studi.Api.Proctoring.Services
{
    public class ExamSessionService : IExamSessionService
    {
        /// <summary>
        /// Defines the services.
        /// </summary>
        private readonly IExamSessionRepository examSessionRepository;
        private readonly IGlobalSettingService globalSettingService;

        public ExamSessionService(IServiceProvider serviceProvider)
        {
            examSessionRepository = serviceProvider.ExamSessionRepository();
            globalSettingService = serviceProvider.GlobalSettingService();
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
        /// GetExamSession using evaluationCode, promotionCode, and filiereCode
        /// </summary>
        /// <param name="evaluationCode"></param>
        /// <param name="promotionCode"></param>
        /// <param name="filiereCode"></param>
        /// <returns></returns>
        public async Task<ExamSessionDto> GetExamSessionAsync(string evaluationCode, string promotionCode, string filiereCode)
        {
            return await examSessionRepository.GetExamSessionAsync(evaluationCode, promotionCode, filiereCode);
        }

        /// <summary>
        /// RetrieveExamSessionFromLmsAndSave
        /// </summary>
        /// <param name="ressourceVersionId"></param>
        /// <param name="promotionCode"></param>
        /// <param name="filiereCode"></param>
        /// <param name="authToken"></param>
        public async Task<ExamSessionDto> CreateExamSessionWithInfosFromLmsAsync(int ressourceVersionId, string promotionCode, string filiereCode, string authToken, ILmsApiClient lmsApiClient)
        {
            // Set authorization token to be allowed to request the Lms.Api
            lmsApiClient.AuthToken = authToken;

            // Get exam's infos from LMS
            var examSessionInfosFromLms = lmsApiClient.GetExamInfos(ressourceVersionId, promotionCode, filiereCode);
            if (examSessionInfosFromLms is null)
                throw new Exception($"Unable to retrieve exam version n°{ressourceVersionId} from LMS.API");

            // Set proctoringImages interval from GlobalSettings
            var examSessionProctoringImagesInterval = (await globalSettingService.GetProctoringImageIntervalAsync()).Value;
            examSessionInfosFromLms.IntervalProctoringImages = Convert.ToInt32(examSessionProctoringImagesInterval);

            var newExamSessionInfos = await examSessionRepository.CreateExamSessionAsync(examSessionInfosFromLms);
            return newExamSessionInfos;
        }
    }
}
