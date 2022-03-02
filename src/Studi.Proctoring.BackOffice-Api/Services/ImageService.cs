using System;
using Microsoft.AspNetCore.Http;
using System.IO;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.BackOffice_Api.Helpers;
using Studi.Proctoring.Database.Context;
using System.Threading.Tasks;
using Studi.Proctoring.BackOffice_Api.Repositories;
using System.Collections.Generic;

namespace Studi.Proctoring.BackOffice_Api.Services
{

    public class ImageService : IImageService
    {
        /// <summary>
        /// Defines the services.
        /// </summary>
        private readonly IImageRepository imageRepository;
        private readonly IProctoringRepository proctoringRepository;

        public ImageService(IServiceProvider serviceProvider)
        {
            imageRepository = serviceProvider.ImageRepository();
            proctoringRepository = serviceProvider.ProctoringRepository();
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
