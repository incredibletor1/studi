using Studi.Proctoring.BackOffice_Api.Helpers;
using Studi.Proctoring.BackOffice_Api.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Services
{
    public class HangFireService : IHangFireService
    {
        /// <summary>
        /// Defines the services.
        /// </summary>
        private readonly IImageService imageService;
        private readonly IUserExamSessionService userExamSessionService;
        private readonly IExamSessionService examSessionService;

        private const string _deleteUserName = "Hangfire Obsolete Deletion";
        private const string _finishWithErrorUserName = "Hangfire UserExamSessions FinishWithError";

        public HangFireService(IServiceProvider serviceProvider)
        {
            imageService = serviceProvider.ImageService();
            userExamSessionService = serviceProvider.UserExamSessionService();
            examSessionService = serviceProvider.ExamSessionService();
        }

        /// <summary>
        /// DeleteAllObsoleteExamSessionsAndUserExamSessions
        /// </summary>
        public async Task<List<int>> DeleteAllObsoleteExamSessionsAndUserExamSessions()
        {
            // Retrieve all obsolete users' exams to delete 
            var userExamSessionsToDelete = await userExamSessionService.GetObsoleteUserExamSessionsToDeleteAsync();

            // Mark as deleted in database and delete files of proctoring and user checks images for obsolete users' exams
            await imageService.DeleteAllImagesForUserExamsAsync(userExamSessionsToDelete, _deleteUserName);

            // Flag as deleted in database obsolete users' exams
            await userExamSessionService.DeleteUserExamSessionsAsync(userExamSessionsToDelete, _deleteUserName);

            // Flag as deleted in database obsolete exams (with only deleted users' exams)
            var deletedExamSessionsIds = await examSessionService.DeleteObsoleteExamSessionsAsync(_deleteUserName);

            return deletedExamSessionsIds;
        }

        /// <summary>
        /// UpdateWronglyOngoingUserExamsToFinishWithErrorStatus
        /// </summary>
        public async Task<List<int>> UpdateWronglyOngoingUserExamsToFinishWithErrorStatusAsync()
        {
            // Retrieve all userExamSessions to FinishWithError
            var userExamSessionsToFinishWithError = await userExamSessionService.GetUserExamSessionsToFinishWithErrorStatusAsync();

            // Flag userExamSessions as FinishedWithError in database 
            var finishedWithErrorUserExamSessionsIds = await userExamSessionService.UpdateUserExamsStatusToFinishWithErrorAsync(userExamSessionsToFinishWithError, _finishWithErrorUserName);

            return finishedWithErrorUserExamSessionsIds;
        }
    }
}
