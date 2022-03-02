using Studi.Api.Proctoring.Models.VM;
using System;
using System.Net.Http;
using Studi.Api.Proctoring.Helpers;
using System.Threading.Tasks;
using Studi.Api.Proctoring.Models.DTO;
using Studi.Proctoring.Database.Context;
using Microsoft.Extensions.Options;
using Studi.Api.Proctoring.Models;
using Studi.Api.Proctoring.Repositories;
using System.Collections.Generic;

namespace Studi.Api.Proctoring.Services
{
    public class ProctoringService : IProctoringService
    {
        /// <summary>
        /// Defines the services.
        /// </summary>
        public readonly IHttpClientFactory _clientFactory;
        private readonly IOptions<FeatureFlipping> _featureFlippingConfig;
        private readonly IUserService userService;
        private readonly IExamSessionService examSessionService;
        private readonly IUserExamSessionService userExamSessionService;
        private readonly IImageService imageService;
        private readonly ICacheService cacheService;
        private ILmsApiClient lmsApiClientService;

        public ProctoringService(IServiceProvider serviceProvider, IHttpClientFactory clientFactory, IOptions<FeatureFlipping> featureFlippingConfig)
        {
            userService = serviceProvider.UserService();
            examSessionService = serviceProvider.ExamSessionService();
            userExamSessionService = serviceProvider.UserExamSessionService();
            lmsApiClientService = serviceProvider.LmsApiClientService();
            imageService = serviceProvider.ImageService();
            cacheService = serviceProvider.CacheService();
            _clientFactory = clientFactory;
            _featureFlippingConfig = featureFlippingConfig;
        }

        /// <summary>
        /// Create or get User and make token
        /// </summary>
        /// <param name="startExamVM"></param>
        /// <param name="lmsApiBaseUrl"></param>
        /// <returns>returns token, throw exception if invalid params</returns>
        public async Task<int> InitUserExamAndCreateIDCheckImagesAsync(StartExamVM startExamVM, string lmsApiBaseUrl, string authToken)
        {
            if (startExamVM == null)
                throw new NullReferenceException("no object/incorrect object");
            else if (string.IsNullOrWhiteSpace(startExamVM.Email))
                throw new NullReferenceException("userEmail haven't been provided");
            else if(string.IsNullOrWhiteSpace(startExamVM.FiliereCode))
                throw new NullReferenceException("FiliereCode haven't been provided");
            else if (string.IsNullOrWhiteSpace(startExamVM.PromotionCode))
                throw new NullReferenceException("PromotionCode haven't been provided");
            else if (string.IsNullOrWhiteSpace(startExamVM.EvaluationCode))
                throw new NullReferenceException("EvaluationCode haven't been provided");
            else if (string.IsNullOrWhiteSpace(lmsApiBaseUrl))
                throw new NullReferenceException("lmsApiBaseUrl haven't been retrieved properly from caller");
            else
            {             
                // Set LMS.API base host URI for Lms.Api calls
                lmsApiClientService.LmsApiBaseUrl = lmsApiBaseUrl;

                // Retrieve user or create it if not yet known from proctoring DB
                var user = await userService.GetUserByEmailAsync(startExamVM.Email);
                if (user is null)
                {
                    user = await userService.CreateUserWithInfosFromLmsAsync(startExamVM.EvaluationCode, authToken, lmsApiClientService);
                }

                // Retrieve exam session or create it if not yet known from proctoring DB
                var examSession = await examSessionService.GetExamSessionAsync(startExamVM.EvaluationCode, startExamVM.PromotionCode, startExamVM.FiliereCode);
                if (examSession is null)
                {
                    examSession = await examSessionService.CreateExamSessionWithInfosFromLmsAsync(startExamVM.EvaluationVersionId, startExamVM.PromotionCode, startExamVM.FiliereCode, authToken, lmsApiClientService);
                }

                // Create user's exam session & save user check images if not yet exist in DB
                var userExamSession = await userExamSessionService.GetUserExamSessionAsync(user.LmsUserId, startExamVM.EvaluationCode, startExamVM.PromotionCode, startExamVM.FiliereCode);
                if (userExamSession is null)
                {
                    // Retrieve ID image type from provided id
                    var idImageType = (IDTypeEnum?)startExamVM.PhotoIdType;

                    // Create user's exam session
                    userExamSession = await userExamSessionService.CreateUserExamSessionWithInfosFromLmsAsync(user.LmsUserId, examSession, idImageType, startExamVM.Photo != null, startExamVM.PhotoId != null, authToken, lmsApiClientService);

                    // Save both user check images (ID + user picture) - TODO: could be outside the if with proper checks
                    //if (startExamVM.PhotoId != null)
                        await imageService.CreateUserStartExamImageAsync(userExamSession, startExamVM.PhotoId, ImageTypeEnum.UserId);
                    //if (startExamVM.Photo != null)
                        await imageService.CreateUserStartExamImageAsync(userExamSession, startExamVM.Photo, ImageTypeEnum.UserImage);
                }

                if (startExamVM.PhotoId != null)
                await imageService.CreateUserStartExamImageAsync(userExamSession, startExamVM.PhotoId, ImageTypeEnum.UserId);
                if (startExamVM.Photo != null)
                await imageService.CreateUserStartExamImageAsync(userExamSession, startExamVM.Photo, ImageTypeEnum.UserImage);

                return userExamSession.Id;
            }
        }

        /// <summary>
        /// Upload Proctoring Image
        /// </summary>
        /// <param name="uploadImage"></param>
        /// <param name="lmsUserId"></param>
        /// <returns></returns>
        public async Task UploadProctoringImageAsync(UploadImageVM uploadImage, int lmsUserId)
        {
            // Retrieve user id from lmsUserId (from cache if available),
            var userId = await cacheService.GetUserIdByLmsUserIdAsync(lmsUserId);

            // Retrieve user's exam id from: [lmsUserId, evaluationCode, promotionCode, filiereCode] (from cache if available).
            var userExamSessionId = await cacheService.GetUserExamSessionIdIfOngoingAsync(lmsUserId, uploadImage.EvaluationCode, uploadImage.PromotionCode, uploadImage.FiliereCode);

            // For now, check that image file size doesn't exceed a 10Mo max. size
            if (uploadImage.ImageFile.Length > Math.Pow(10, 7)) // TODO: later on, use the same param front use to define image size to reject image exceeding this max size
                throw new ArgumentException("The provided proctoring image size is beyond the maximum accepted size");

            // Check that this very request doesn't arrive before the parameterized interval between two proctoring images elapsed (to avoid DDoS attack)
            if (_featureFlippingConfig.Value?.ActivateCheckImageIntervalElapsed == true)
            {
                var lastProctoringImageTimestamp = await cacheService.GetLastImageTimestampByUserExamId(userExamSessionId);
                if ((DateTime.Now - lastProctoringImageTimestamp).TotalSeconds < 2) // Hardcoded 2 sec. independently from concrete interval between 2 proctoring images as the goal is just to avoid DDoS attacks
                    throw new ArgumentException("A new proctoring image cannot be handled by the server before the expected time interval has elapsed");
            }

            // Synchronously process and save the proctoring image (to avoid server crashes on overload/DDoS)
            await imageService.CreateUserProctoringImageAsync(userId, userExamSessionId, uploadImage.ImageFile);
        }

        /// <summary>
        /// End user's exam session
        /// </summary>
        /// <param name="lmsUserId"></param>
        /// <param name="evaluationCode"></param>
        /// <param name="promotionCode"></param>
        /// <param name="filiereCode"></param>
        public async Task FinishUserExamAsync(int lmsUserId, string evaluationCode, string promotionCode, string filiereCode)
        {
            var userExamSession = await userExamSessionService.GetUserExamSessionAsync(lmsUserId, evaluationCode, promotionCode, filiereCode);
            if (userExamSession is null)
                throw new ArgumentException("Cannot find user's exam with provided parameters");

            await userExamSessionService.EndUserExamSessionAsync(userExamSession.Id);

            // Remove related keys from cache (avoid uploading image being still possible)
            cacheService.RemoveFromCacheUserIdByLmsUserId(lmsUserId);
            cacheService.RemoveFromCacheUserExamSessionId(lmsUserId, evaluationCode, promotionCode, filiereCode);
            cacheService.RemoveFromCacheImageMaxOrderByUserExamId(userExamSession.Id);
        }

        public async Task<int?> UserExamMaterialCheckAndStartUserExamAsync(UserExamMaterialCheckDto userExamMaterialCheckDto)
        {
            var updatedUserExamSessionId = (await userExamSessionService.UpdateMaterialCheckAndStartUserExamAsync(userExamMaterialCheckDto)).SessionExamId;

            // Retrieve proctoringImages interval for current UserExamSession
            var currentUserExamSessionProctoringImagesInterval = (await examSessionService.GetExamSessionByIdAsync(updatedUserExamSessionId)).IntervalProctoringImages;

            return currentUserExamSessionProctoringImagesInterval;
        }
    }
}
