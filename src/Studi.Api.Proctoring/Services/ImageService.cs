using System;
using Microsoft.AspNetCore.Http;
using System.IO;
using Studi.Api.Proctoring.Models.DTO;
using Studi.Api.Proctoring.Helpers;
using Studi.Proctoring.Database.Context;
using System.Threading.Tasks;
using Studi.Api.Proctoring.Repositories;
using System.Collections.Generic;

namespace Studi.Api.Proctoring.Services
{

    public class ImageService : IImageService
    {
        /// <summary>
        /// Defines the services.
        /// </summary>
        private readonly IUserService userService;
        private readonly IImageRepository imageRepository;
        private readonly IProctoringRepository proctoringRepository;
        private readonly IUserExamSessionRepository userExamSessionRepository;
        private readonly ICacheService cacheService;
        private readonly IUserExamSessionService userExamSessionService;

        public ImageService(IServiceProvider serviceProvider)
        {
            userService = serviceProvider.UserService();
            imageRepository = serviceProvider.ImageRepository();
            proctoringRepository = serviceProvider.ProctoringRepository();
            userExamSessionRepository = serviceProvider.UserExamSessionRepository();
            cacheService = serviceProvider.CacheService();
            userExamSessionService = serviceProvider.UserExamSessionService();
        }

        /// <summary>
        /// Upload either a user's identification document (ID) or user's picture image provided at exam's begining
        /// </summary>
        /// <param name="userExamSession"></param>
        /// <param name="photo"></param>
        /// <param name="imageType"></param>
        /// <returns></returns>
        public async Task CreateUserStartExamImageAsync(UserExamSessionDto userExamSession, IFormFile photo, ImageTypeEnum imageType)
        {
            if (userExamSession is null)
                throw new NullReferenceException("userExamSession wasn't found in database");
            
            if (imageType != ImageTypeEnum.UserId && imageType != ImageTypeEnum.UserImage)
                throw new ArgumentException($"{nameof(CreateUserStartExamImageAsync)} method is meant solely for uploading ID or user's images, not: '{imageType.ToString()}'.");

            var user = await userService.GetUserByIdAsync(userExamSession.UserId);
            if (user is null)
                throw new NullReferenceException($"User with id: '{userExamSession.UserId}' wasn't found in database");

            var containerType = ContainerTypeEnum.FileSystem;

            // Save picture to file system (or whatever storage is configured)
            var userPicture = ConvertImageToByte(photo);
            var containerPhoto = await imageRepository.CreateUserCheckImageAsync(user.Id, userExamSession.Id, DateTime.Now, imageType, userPicture);

            if (string.IsNullOrWhiteSpace(containerPhoto))
                throw new NullReferenceException("containerPhoto or containerTypeId = null");

            // Save picture to proctoring database
            var newImageDto = new UserImageCheckDto
            {
                UserExamSessionId = userExamSession.Id,
                ImageTypeId = (int)imageType,
                ImageTimeStamp = DateTime.Now,
                ContainerImageId = containerPhoto,
                ContainerTypeId = GetContainerType(containerType),
            };
            await proctoringRepository.CreateUserCheckImageAsync(newImageDto);
        }

        /// <summary>
        /// Save user's proctoring image 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userExamSessionId"></param>
        /// <param name="imageFile"></param>
        public async Task CreateUserProctoringImageAsync(int userId, int userExamSessionId, IFormFile imageFile)
        {
            // No need for data consistency tests here, as they have been made prior

            // Update the cache last image timestamp value to the current image timestamp 
            var imageTimestamp = DateTime.Now; 
            await cacheService.UpdateCacheLastImageTimestampByUserExamId(userExamSessionId, imageTimestamp);
            
            var userPicture = ConvertImageToByte(imageFile);

            // Save the provided proctoring image file
            var containerPhoto = await imageRepository.CreateProctoringImageAsync(userId, userExamSessionId, DateTime.Now, userPicture);
            var containerType = ContainerTypeEnum.FileSystem;
            var containerTypeId = GetContainerType(containerType);
            
            // Calculate the next order with which creating the image
            var maxOrder = await cacheService.GetImageMaxOrderByUserExamId(userExamSessionId);
            var nextOrder = maxOrder + 1;

            // Add reference to the proctoring image into database
            var proctoringImage = new ProctoringImageDto
            {
                UserExamSessionId = userExamSessionId,
                Order = nextOrder,
                ImageTimeStamp = imageTimestamp,
                ContainerImageId = containerPhoto,
                ContainerTypeId = containerTypeId,
            };
            await proctoringRepository.CreateProctoringImageAsync(proctoringImage);

            // Update the cache maxOrder value to nextOrder 
            await cacheService.UpdateCacheImageMaxOrderByUserExamId(userExamSessionId, nextOrder);
        }

        /// <summary>
        /// Check for contaiter type existence
        /// </summary>
        /// <param name="containerType">current container type</param>
        /// <returns>returns the containerType id, throw exception if not found</returns>
        public int GetContainerType(ContainerTypeEnum containerType)
        {
            if (containerType != ContainerTypeEnum.FileSystem)
                throw new Exception($"{containerType} container isn't handled");
            else
            {
                return (int)containerType;
            }
        }

        /// <summary>
        /// Convert image to bytes array
        /// </summary>
        /// <param name="image">image file</param>
        /// <returns>The byte massive that represents image</returns>
        public byte[] ConvertImageToByte(IFormFile image)
        {
            byte[] somePicture = null;
            using (var binaryReader = new BinaryReader(image.OpenReadStream()))
            {
                somePicture = binaryReader.ReadBytes((int)image.Length);
            }
            return somePicture;
        }
    }
}
