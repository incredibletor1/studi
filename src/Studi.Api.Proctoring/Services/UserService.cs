using System;
using System.Threading.Tasks;
using Studi.Api.Proctoring.Helpers;
using Studi.Api.Proctoring.Models.DTO;
using Studi.Api.Proctoring.Repositories;

namespace Studi.Api.Proctoring.Services
{
    public class UserService : IUserService
    {
        /// <summary>
        /// Defines the services.
        /// </summary>
        private readonly IUserRepository userRepository;

        public UserService(IServiceProvider serviceProvider)
        {
            userRepository = serviceProvider.UserRepository();
        }

        /// <summary>
        /// Get a user by its email
        /// </summary>
        /// <param name="email">the user email</param>
        /// <returns>returns the corresponding user</returns>
        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await userRepository.GetUserByEmailAsync(email); 
            return user;
        }

        /// <summary>
        /// Get a user by its id
        /// </summary>
        /// <param name="id">the user id</param>
        /// <returns>returns the corresponding user</returns>
        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await userRepository.GetUserByIdAsync(userId);
            return user;
        }

        /// <summary>
        /// Get a user by its id
        /// </summary>
        /// <param name="id">the user id</param>
        /// <returns>returns the corresponding user</returns>
        public async Task<UserDto> GetUserByLmsUserIdAsync(int lmsUserId)
        {
            var user = await userRepository.GetUserByLmsUserIdAsync(lmsUserId);
            return user;
        }

        /// <summary>
        /// RetrieveUserFromLmsAndSave
        /// </summary>
        /// <param name="evaluationCode"></param>
        /// <param name="authToken"></param>
        /// <returns></returns>
        public async Task<UserDto> CreateUserWithInfosFromLmsAsync(string evaluationCode, string authToken, ILmsApiClient lmsApiClient)
        {
            // Set authorization token to be allowed to request the Lms.Api
            lmsApiClient.AuthToken = authToken;

            // Get user's infos from LMS
            var userInfos = lmsApiClient.GetUserInfos(evaluationCode);
            if (userInfos is null)
                throw new Exception($@"Unable to retrieve user from LMS.API for evaluation/ressource with code n°{evaluationCode}");

            // Save the new user
            var newUserInfos = await userRepository.CreateUserAsync(userInfos);
            return newUserInfos;
        }
    }
}
