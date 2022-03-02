using Studi.Api.Proctoring.Helpers;
using Studi.Api.Proctoring.Models.DTO;
using Studi.Api.Proctoring.Repositories;
using Studi.Proctoring.Database.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Studi.Api.Proctoring.Services
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
        /// Get a userExamSession by its id
        /// </summary>
        /// <param name="id">the userExamSession id</param>
        /// <returns>returns the corresponding userExamSession</returns>
        public async Task<UserExamSessionDto> GetUserExamSessionByIdAsync(int id)
        {
            return await userExamSessionRepository.GetUserExamSessionByIdAsync(id);
        }

        /// <summary>
        /// GetUserExamSessionId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="evaluationCode"></param>
        /// <returns></returns>
        public async Task<UserExamSessionDto> GetUserExamSessionAsync(int lmsUserId, string evaluationCode, string promotionCode, string filiereCode)
        {
            return await userExamSessionRepository.GetUserExamSessionAsync(lmsUserId, evaluationCode, promotionCode, filiereCode);
        }

        /// <summary>
        /// RetrieveUserExamSessionFromLmsAndSave
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="examSession"></param>
        /// <param name="IDimageType"></param>
        /// <param name="hasUserPictureBeenProvided"></param>
        /// <param name="hasUserIdentityDocBeenProvided"></param>
        /// <param name="authToken"></param>
        /// <returns>the retrieved user's exam session</returns>
        public async Task<UserExamSessionDto> CreateUserExamSessionWithInfosFromLmsAsync(int lmsUserId, ExamSessionDto examSession, IDTypeEnum? IDimageType, bool hasUserPictureBeenProvided, bool hasUserIdentityDocBeenProvided, string authToken, ILmsApiClient lmsApiClient)
        {
            // Check if user's exam already exists (do nothing case)
            var existingUserExam = await userExamSessionRepository.GetUserExamSessionAsync(lmsUserId, examSession.RessourceCode, examSession.PromotionCode, examSession.FiliereCode);
            if (existingUserExam != null)
                return existingUserExam;

            // Check if user and exam really exist yet
            var existingUser = await userService.GetUserByLmsUserIdAsync(lmsUserId);
            if (existingUser is null)
                throw new ArgumentException($"Cannot create user's exam because user with lmsUserId= {lmsUserId} was not found in database");

            if (await examSessionService.GetExamSessionByIdAsync(examSession.Id) is null)
                throw new ArgumentException($"Cannot create user's exam because examSession with Id= {examSession.Id} was not found in database");

            // Set authorization token to be allowed to request the Lms.Api
            lmsApiClient.AuthToken = authToken;

            // Retrieve user's infos for exam session from LMS
            var userExamSessionInfosFromLms = lmsApiClient.GetUserExamInfos(examSession.RessourceCode, examSession.PromotionCode, existingUser.Id, examSession.Id);
            if (userExamSessionInfosFromLms is null)
                throw new ArgumentException($"Unable to retrieve user's infos for user with id {existingUser.Id} and exam session with id n°{examSession.Id} from LMS.API");

            userExamSessionInfosFromLms.StatusId = (int)ExamStatusEnum.Ongoing;
            userExamSessionInfosFromLms.IdentityDocumentType = (IDimageType is null) ? null : (int?)IDimageType.Value;
            userExamSessionInfosFromLms.HasUserPictureBeenProvided = hasUserPictureBeenProvided;
            userExamSessionInfosFromLms.HasUserIdentityDocBeenProvided = hasUserIdentityDocBeenProvided;

            // Save user's exam session
            var newUserExamSessionInfos = await userExamSessionRepository.CreateUserExamSessionAsync(userExamSessionInfosFromLms);
            return newUserExamSessionInfos;
        }

        /// <summary>
        /// End specified user's exam session
        /// </summary>
        /// <param name="examSessionId">the exam session id</param>
        /// <param name="actualEndTime">the actualEndTime of exam</param>
        public async Task EndUserExamSessionAsync(int userExamSessionId)
        {
            // Fetch existing user exam session
            var userExamSession = await this.GetUserExamSessionByIdAsync(userExamSessionId);
            if (userExamSession is null)
                throw new ArgumentException($"user exam session with id n°{userExamSessionId} was not found in database");

            if (userExamSession.StatusId != (int)ExamStatusEnum.Ongoing)
                throw new ArgumentException($"user exam session with id n°{userExamSessionId} is already finished");

            // Update user's exam session infos and mark it as finished
            userExamSession.StatusId = (int)ExamStatusEnum.Finished;
            userExamSession.ActualEndTime = DateTime.Now;
            if (userExamSession.ActualStartTime != null)
            {
                userExamSession.ExamActualDuration = Convert.ToInt32(
                    Math.Truncate((userExamSession.ActualEndTime - userExamSession.ActualStartTime).Value.TotalMinutes));
            }

            // Save changes
            await userExamSessionRepository.UpdateUserExamSessionAsync(userExamSession);
        }

        public async Task<UserExamSessionDto> UpdateMaterialCheckAndStartUserExamAsync(UserExamMaterialCheckDto userExamMaterialCheckDto)
        {
            var userExamSession = await userExamSessionRepository.GetUserExamSessionByIdAsync(userExamMaterialCheckDto.UserExamSessionId);

            userExamSession.HasUserConnectionBeenTested = userExamMaterialCheckDto.HasUserConnectionBeenTested;
            userExamSession.HasUserIdentityDocBeenProvided = userExamMaterialCheckDto.HasUserIdentityDocBeenProvided;
            userExamSession.HasUserPictureBeenProvided = userExamMaterialCheckDto.HasUserPictureBeenProvided;
            userExamSession.IdentityDocumentType = userExamMaterialCheckDto.IdentityDocumentType;
            userExamSession.UploadSpeedTest = userExamMaterialCheckDto.UploadSpeedTest;
            userExamSession.DownloadSpeedTest = userExamMaterialCheckDto.DownloadSpeedTest;
            userExamSession.HasMicrophone = userExamMaterialCheckDto.HasMicrophone;
            userExamSession.HasWebcam = userExamMaterialCheckDto.HasWebcam;
            userExamSession.UserInfrastructure = userExamMaterialCheckDto.UserInfrastructure;
            // Start UserExam
            userExamSession.ActualStartTime = DateTime.Now;

            await userExamSessionRepository.UpdateUserExamSessionAsync(userExamSession);
            return userExamSession;
        }
    }
}
