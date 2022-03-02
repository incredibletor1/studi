using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using Studi.Proctoring.Database.Context;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Collections.Generic;
using Studi.Proctoring.BackOffice_Api.Models;
using System.Globalization;
using CsvHelper;

namespace Studi.Proctoring.BackOffice_Api.Repositories
{
    public class FSImageRepository : IImageRepository
    {
        protected string _userIDsRootDirectory;     // Path to the root directory for the user identidication images, such as User and ID card photos
        protected string _proctoringRootDirectory;  // Path to the root directory for the proctoring images
        protected string _downloadRootDirectory;    // Path to the root directory for downloading images (temporary storage)

        public FSImageRepository(IConfiguration config)
        {
            try
            {
                DirectoryInfo infoUser = new DirectoryInfo(config.GetValue<string>("UserIDsRootDirectory"));
                DirectoryInfo infoProctoring = new DirectoryInfo(config.GetValue<string>("ProctoringRootDirectory"));
                DirectoryInfo infoDownload = new DirectoryInfo(config.GetValue<string>("DownloadRootDirectory"));
                _userIDsRootDirectory = infoUser.FullName;
                _proctoringRootDirectory = infoProctoring.FullName;
                _downloadRootDirectory = infoDownload.FullName;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete all proctoring images for specified userExam 
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="userExamId">userExamSession id</param>
        /// <returns></returns>
        public void DeleteAllProctoringImagesFilesByUserExam(int userId, int userExamId)
        {
            var imageRootDirectory = GetProctoringImagesContainerPath();
            if (string.IsNullOrWhiteSpace(imageRootDirectory))
                throw new DirectoryNotFoundException("Root directory of the image container is not set.");

            DeleteImagesWithinFolder(userId, userExamId, imageRootDirectory);
        }

        /// <summary>
        /// Delete all user's CheckImages for specified userExam 
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="userExamId">userExamSession id</param>
        /// <returns></returns>
        public void DeleteAllUserCheckImagesFilesByUserExam(int userId, int userExamId)
        {
            var imageRootDirectory = GetUserImageCheckContainerPath();
            if (string.IsNullOrWhiteSpace(imageRootDirectory))
                throw new DirectoryNotFoundException("Root directory of the image container is not set.");

            DeleteImagesWithinFolder(userId, userExamId, imageRootDirectory);
        }

        /// <summary>
        /// Delete all images within specified directory
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="userExamId">userExamSession id</param>
        /// <param name="imageRootDirectory">image type root direction</param>
        /// <returns></returns>
        private void DeleteImagesWithinFolder(int userId, int userExamId, string imageRootDirectory)
        {
            string directoryPath = Path.Combine(
                imageRootDirectory,
                string.Format(@"user_{0}", userId),
                string.Format(@"session_{0}", userExamId));

            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }
        }

        /// <summary>
        /// The method returns the path to the container root directory depending on the image type.
        /// </summary>
        /// <param name="imageType">Type of the image for which the root directory should be selected.</param>
        /// <returns>Path to the image container root directory.</returns>
        protected string GetUserImageCheckContainerPath()
        {
            return _userIDsRootDirectory;
        }

        protected string GetProctoringImagesContainerPath()
        {
            return _proctoringRootDirectory;
        }

        protected string GetDownloadImagesContainerPath()
        {
            return _downloadRootDirectory;
        }

        public async Task<string> GetExamSessionPathForDownloadAsync(int examSessionId, IEnumerable<UserExamForDownloadDto> userExamSessionsInfo, string examSessionCsvPath)
        {
            var proctoringImagesRootDirectory = GetProctoringImagesContainerPath();
            var userCheckImagesRootDirectory = GetUserImageCheckContainerPath();
            var downloadImagesRootDirectory = GetDownloadImagesContainerPath(); 

            if (userCheckImagesRootDirectory is null || proctoringImagesRootDirectory is null || downloadImagesRootDirectory is null)
                throw new DirectoryNotFoundException("Root directory of the image container is not set.");

            if (!Directory.Exists(downloadImagesRootDirectory))
                Directory.CreateDirectory(downloadImagesRootDirectory);

            string temporaryStoragePath = Path.Combine(
                downloadImagesRootDirectory,
                string.Format("UserExamSessionZipsForDownload"));

            string temporaryUserZipStoragePath = Path.Combine(
                downloadImagesRootDirectory,
                string.Format("UserZipsForDownload"));

            foreach (var info in userExamSessionsInfo)
            {
                string proctoringImagesDirectoryPath = Path.Combine(
                    proctoringImagesRootDirectory,
                    string.Format(@"user_{0}", info.UserId),
                    string.Format(@"session_{0}", info.UserExamId));

                string userCheckImagesDirectoryPath = Path.Combine(
                    userCheckImagesRootDirectory,
                    string.Format(@"user_{0}", info.UserId),
                    string.Format(@"session_{0}", info.UserExamId));

                string userZipName = Path.Combine(
                    temporaryUserZipStoragePath,
                    string.Format(@"user_{0}.zip", info.UserId));

                string userSessionZipName = Path.Combine(
                    temporaryStoragePath,
                    string.Format(@"session_{0}.zip", info.UserExamId));

                await CreateCsvFileForUser(info.UserId, info.UserEmail, info.UserFirstName, info.UserLastName);

                using (FileStream zipToOpen = new FileStream(userSessionZipName, FileMode.Create))
                {
                    using (ZipArchive userExamSessionArchive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                    {
                        if (Directory.Exists(proctoringImagesDirectoryPath))
                        {
                            foreach (var imagePath in Directory.GetFiles(proctoringImagesDirectoryPath))
                            {

                                ZipArchiveEntry image = userExamSessionArchive.CreateEntryFromFile(imagePath, Path.GetFileName(imagePath));
                            }
                        }

                        if (Directory.Exists(userCheckImagesDirectoryPath))
                        {
                            foreach (var imagePath in Directory.GetFiles(userCheckImagesDirectoryPath))
                            {
                                ZipArchiveEntry image = userExamSessionArchive.CreateEntryFromFile(imagePath, Path.GetFileName(imagePath));
                            }
                        }
                    }
                }

                await Task.Run(() => ZipFile.CreateFromDirectory(temporaryStoragePath, userZipName));
            }

            string zipFolderPath = Path.Combine(
                downloadImagesRootDirectory,
                string.Format("ResultForDownload"));

            string zipPath = Path.Combine(
                zipFolderPath,
                string.Format(@"ExamSession_{0}.zip", examSessionId));

            if (Directory.Exists(zipFolderPath))
                Directory.Delete(zipFolderPath, true);

            Directory.CreateDirectory(zipFolderPath);

            await Task.Run(() => ZipFile.CreateFromDirectory(temporaryUserZipStoragePath, zipPath));

            return zipPath;
        }

        public async Task<string> CreateCsvFileForUser(int userId, string userEmail, string userFirstName, string userLastName)
        {
            var downloadImagesRootDirectory = GetDownloadImagesContainerPath();

            string temporaryUserZipStoragePath = Path.Combine(
                downloadImagesRootDirectory,
                string.Format("UserExamSessionZipsForDownload"));

            if (Directory.Exists(temporaryUserZipStoragePath))
                Directory.Delete(temporaryUserZipStoragePath, true);

            var csvPath = Path.Combine(
                temporaryUserZipStoragePath,
                string.Format(@"User_{0}.csv", userId));

            Directory.CreateDirectory(temporaryUserZipStoragePath);

            var userCsv = new UserCsv
            {
                FirstName = userFirstName,
                LastName = userLastName,
                Email = userEmail
            };

            using (StreamWriter streamWriter = new StreamWriter(csvPath))
            {
                using (CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteHeader<UserCsv>();
                    csvWriter.NextRecord();
                    csvWriter.WriteRecord(userCsv);
                }
            }

            return csvPath;
        }

        public async Task<string> CreateCsvFileForExamSession(ExamSessionView examSession)
        {
            var downloadImagesRootDirectory = GetDownloadImagesContainerPath();

            var temporaryStoragePath = Path.Combine(
                downloadImagesRootDirectory,
                string.Format("UserZipsForDownload"));

            if (Directory.Exists(temporaryStoragePath))
                Directory.Delete(temporaryStoragePath, true);

            var csvPath = Path.Combine(
                temporaryStoragePath,
                string.Format(@"ExamSession_{0}.csv", examSession.Id));

            Directory.CreateDirectory(temporaryStoragePath);

            var examSessionCsv = new ExamSessionCsv
            {
                RessourceName = examSession.RessourceName,
                EvalBlockName = examSession.EvalBlockName,
                ParcoursName = examSession.ParcoursName,
                PromotionName = examSession.PromotionName,
                FiliereName = examSession.FiliereName,
                ScheduledBeginStartTime = examSession.ScheduledBeginStartTime,
                ScheduledEndStartTime = examSession.ScheduledEndStartTime,
                UsersCount = examSession.UsersCount,
                ExamStatus = examSession.ExamStatus
            };

            using (StreamWriter streamWriter = new StreamWriter(csvPath))
            {
                using (CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    csvWriter.WriteHeader<ExamSessionCsv>();
                    csvWriter.NextRecord();
                    csvWriter.WriteRecord(examSessionCsv);
                }
            }

            return csvPath;
        }
    }
}
