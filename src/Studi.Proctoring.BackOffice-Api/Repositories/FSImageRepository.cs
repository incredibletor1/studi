using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using Studi.Proctoring.Database.Context;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Repositories
{
    public class FSImageRepository : IImageRepository
    {
        protected string _userIDsRootDirectory;     // Path to the root directory for the user identidication images, such as User and ID card photos
        protected string _proctoringRootDirectory;  // Path to the root directory for the proctoring images

        public FSImageRepository(IConfiguration config)
        {
            try
            {
                DirectoryInfo infoUser = new DirectoryInfo(config.GetValue<string>("UserIDsRootDirectory"));
                DirectoryInfo infoProctoring = new DirectoryInfo(config.GetValue<string>("ProctoringRootDirectory"));
                _userIDsRootDirectory = infoUser.FullName;
                _proctoringRootDirectory = infoProctoring.FullName;
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
    }
}
