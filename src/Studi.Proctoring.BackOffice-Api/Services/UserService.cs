using System;
using System.Threading.Tasks;
using Studi.Proctoring.BackOffice_Api.Helpers;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.BackOffice_Api.Repositories;

namespace Studi.Proctoring.BackOffice_Api.Services
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
        /// Get a user by its id
        /// </summary>
        /// <param name="id">the user id</param>
        /// <returns>returns the corresponding user</returns>
        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await userRepository.GetUserByIdAsync(userId);
            return user;
        }
    }
}
