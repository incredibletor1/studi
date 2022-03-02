using System;
using Microsoft.AspNetCore.Http;
using System.IO;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.BackOffice_Api.Helpers;
using Studi.Proctoring.Database.Context;
using System.Threading.Tasks;
using Studi.Proctoring.BackOffice_Api.Repositories;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Studi.Proctoring.BackOffice_Api.Services
{

    public class ImageService : IImageService
    {
        /// <summary>
        /// Defines the services.
        /// </summary>
        private readonly IImageRepository imageRepository;
        private readonly IProctoringRepository proctoringRepository;
        private readonly IUserExamSessionService userExamSessionService;
        private readonly IExamSessionService examSessionService;

        public ImageService(IServiceProvider serviceProvider)
        {
            imageRepository = serviceProvider.ImageRepository();
            proctoringRepository = serviceProvider.ProctoringRepository();
            userExamSessionService = serviceProvider.UserExamSessionService();
            examSessionService = serviceProvider.ExamSessionService();
        }

        public async Task<ExamSessionArchivedDto> GetExamSessionFileInformationByExamSessionIdAsync(int examSessionId)
        {
            var userExamSessions = await userExamSessionService.GetDownloadInformationsAboutUserExamSessionsOfExamSessionByExamSessionIdAsync(examSessionId);

            var examSessionCsvPath = await GetCsvFilePathForExamSession(examSessionId, userExamSessions.Count());

            var path = await imageRepository.GetExamSessionPathForDownloadAsync(examSessionId, userExamSessions, examSessionCsvPath);
            var contentType = "application/zip";
            var fileName = String.Format("ExamSession_{0}.zip", examSessionId);

            return new ExamSessionArchivedDto
            {
                FilePath = path,
                ContentType = contentType,
                FileName = fileName
            };
        }

        public async Task<string> GetCsvFilePathForExamSession(int examSessionId, int usersCount)
        {
            var examSession = await examSessionService.GetExamSessionByIdAsync(examSessionId);

            var examSessionView = examSession.ToEntity().ToView(usersCount, await userExamSessionService.GetExamSessionStatusByExamSessionIdAsync(examSessionId));
            return await imageRepository.CreateCsvFileForExamSession(examSessionView);
        }

        /// <summary>
        /// DeleteAllImagesFilesForObsoleteUserExamAsync
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAllImagesForUserExamsAsync(IEnumerable<UserExamSessionDto> userExamSessionsToDelete, string deleteUserName)
        {            
            foreach (var userExamSession in userExamSessionsToDelete)
            {
                // Mark images as deleted in database
                await proctoringRepository.DeleteFromDBUserCheckPicturesAsync(userExamSession.Id, deleteUserName);
                await proctoringRepository.DeleteFromDBProctoringPicturesAsync(userExamSession.Id, deleteUserName);

                // Delete files
                imageRepository.DeleteAllUserCheckImagesFilesByUserExam(userExamSession.UserId, userExamSession.Id);
                imageRepository.DeleteAllProctoringImagesFilesByUserExam(userExamSession.UserId, userExamSession.Id);
            }
        }
    }
}
