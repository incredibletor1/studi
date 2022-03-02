using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Studi.Proctoring.BackOffice_Api.Helpers;
using Studi.Proctoring.BackOffice_Api.Models;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.BackOffice_Api.Repositories;
using Studi.Proctoring.BackOffice_Api.Repositories.Interfaces;
using Studi.Proctoring.Database.Context;
using static Studi.Proctoring.BackOffice_Api.Models.DTO.AdminUserDTO;

namespace Studi.Proctoring.BackOffice_Api.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IAdminUserRepository adminUserRepository;
        private readonly IPasswordService passwordService;
        private readonly IGlobalSettingRepository globalSettingRepository;
        private readonly ILogger logger;

        public AdminUserService(IServiceProvider services, ILogger<AdminUserService> _logger)
        {
            adminUserRepository = services.AdminUserRepository();
            passwordService = services.PasswordService();
            globalSettingRepository = services.GlobalSettingRepository();
            logger = _logger;
        }

        public async Task<AdminUserDTO> LoginUserAsync(string login, string password)
        {
            var user = await GetUserAsync(login);
            if (user is null)
                return null;

            var passwordHash = passwordService.CalculateHash(password, user.Salt);
            if (user.Password == passwordHash)
                return user;

            return null;
        }

        public async Task<AdminUserDTO> GetUserAsync(string login)
        {
            return await adminUserRepository.GetUserAsync(login);
        }

        public async Task<AdminUserDTO> GetUserByIdAsync(int userId)
        {
            return await adminUserRepository.GetUserByIdAsync(userId);
        }
        public async Task<AdminUsersPageDTO> GetAdminUsersFilteredPaginatedAsync(int PageSize, int PageIndex,
            string searchedString = null, string orderByPropertyName = "Id", SortDirection orderByDirection = SortDirection.OrderByDescending)
        {
            var adminUsersCount = await adminUserRepository.GetCountAdminUsersFilteredAsync(searchedString);
            var adminUsersPage = await adminUserRepository.GetAdminUsersFilteredPaginatedAsync(
                PageSize, PageIndex, searchedString, orderByPropertyName, orderByDirection);

            return new AdminUsersPageDTO
            {
                PageIndex = PageIndex,
                TotalItemsCount = adminUsersCount,
                Page = adminUsersPage
            };
        }

        public async Task<AdminUserDTO> CreateUserAsync(string login, string password, string userCreator)
        {
            if (!passwordService.TestPasswordForRequirements(password))
                throw new ArgumentException("Password does not match the requirements.");

            int passwordExpirationDays = 0;
            var passwordExpirationSetting = await globalSettingRepository.GetPasswordValidityDurationAsync();
            if (!(passwordExpirationSetting is null))
                int.TryParse(passwordExpirationSetting.Value, out passwordExpirationDays);

            var passwordPair = passwordService.GenerateHash(password);
            var newUser = new AdminUserDTO
            {
                Login = login,
                Password = passwordPair.HashB64,
                Salt = passwordPair.SaltB64,
                UserType = AdminUserDTO.UserTypeEnum.User,
                IsActive = true,
                PasswordExpirationDate = (passwordExpirationDays > 0) ? (DateTime.UtcNow + TimeSpan.FromDays(passwordExpirationDays)) : (DateTime?)null
            };

            var adminUser = await adminUserRepository.GetUserAsync(login, true);
            if (adminUser is null)
                return await adminUserRepository.CreateUserAsync(newUser, userCreator);

            // The specified user exists in the database, just marked as deleted, restoring and updating it.
            return await adminUserRepository.RestoreUserAsync(newUser, userCreator);
        }

        public async Task<int> DeleteUserAsync(int userId, string userDeletor)
        {
            return await adminUserRepository.DeleteUserAsync(userId, userDeletor);
        }

        public async Task<List<int>> DeleteUsersAsync(List<int> userIds, string userDeletor)
        {
            var deletedUserIds = new List<int>();

            foreach (var userId in userIds)
            {
                try
                {
                    var deletedUserId = await adminUserRepository.DeleteUserAsync(userId, userDeletor);
                    deletedUserIds.Add(deletedUserId);
                }
                catch (ArgumentException ex)
                {
                    logger.LogError($"Unable to delete admin user with error: {ex}");
                }
            }

            return deletedUserIds;
        }
    }
}
