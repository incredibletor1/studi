using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using Studi.Proctoring.Database.Context;
using Studi.Api.Proctoring.Models.DTO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Studi.Api.Proctoring.Repositories
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
        /// Create proctoring image
        /// </summary>
        /// <param name="userId">current user id</param>
        /// <param name="userExamId">current user exam session id</param>
        /// <param name="timestamp">current timestamp</param>
        /// <param name="imageData">byte massive represents image</param>
        /// <returns>returns the corresponding image path</returns>
        /// <exception cref="DirectoryNotFoundException">Roor directory path not provided in the configuration.</exception>
        /// <exception cref="ArgumentException">Incorrect argument value description text.</exception>
        public async Task<string> CreateProctoringImageAsync(int userId, int userExamId, DateTime timestamp, byte[] imageData)
        {
            var imageRootDirectory = GetProctoringImagesContainerPath();
            if (string.IsNullOrWhiteSpace(imageRootDirectory))
                throw new DirectoryNotFoundException("Root directory of the image container is not set.");

            var filePath = await CreateImageAsync(userId, userExamId, timestamp, imageData, 0, imageRootDirectory);
            return filePath;
        }

        public async Task<string> CreateUserCheckImageAsync(int userId, int userExamId, DateTime timestamp, ImageTypeEnum imageType, byte[] imageData)
        {
            var imageRootDirectory = GetUserImageCheckContainerPath();
            if (string.IsNullOrWhiteSpace(imageRootDirectory))
                throw new DirectoryNotFoundException("Root directory of the image container is not set.");

            var filePath = await CreateImageAsync(userId, userExamId, timestamp, imageData, (int)imageType, imageRootDirectory);
            return filePath;
        }

        private async Task<string> CreateImageAsync(int userId, int userExamId, DateTime timestamp, byte[] imageData, int imageType, string imageRootDirectory)
        {
            if (imageData is null || imageData.Length == 0)
                throw new ArgumentException("Image data not provided.");

            string directoryPath = Path.Combine(
                imageRootDirectory,
                string.Format(@"user_{0}", userId),
                string.Format(@"session_{0}", userExamId));
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            Random rnd = new Random();
            string filePath = Path.Combine(
                directoryPath,
                string.Format(@"{0}_{1}_{2}.jpg", timestamp.ToFileTimeUtc(), imageType, rnd.Next(100, 999)));
            using (FileStream imageFS = File.Create(filePath))
            {
                await imageFS.WriteAsync(imageData);
                imageFS.Close();
            }

            return filePath;
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
