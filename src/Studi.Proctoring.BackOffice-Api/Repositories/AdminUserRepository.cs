using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Studi.Proctoring.BackOffice_Api.Helpers;
using Studi.Proctoring.BackOffice_Api.Models;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Repositories
{
    public class AdminUserRepository : IAdminUserRepository
    {
        private readonly IProctoringContext _proctoringContext;

        public AdminUserRepository(IProctoringContext proctoringContext, IMapper mapper)
        {
            this._proctoringContext = proctoringContext;
            ConversionExtension.InitMapper(mapper);
        }

        public async Task<AdminUserDTO> CreateUserAsync(AdminUserDTO user, string userCreator)
        {
            if (user is null)
                throw new ArgumentException($"User for adding to the repository not set.");

            var userEntity = user.ToEntity();
            userEntity.UserCreate = userCreator;
            userEntity.DateCreate = DateTime.UtcNow;

            _proctoringContext.Add(userEntity);
            await _proctoringContext.SaveChangesAsync();

            return await GetUserAsync(user.Login);
        }

        public async Task<AdminUserDTO> RestoreUserAsync(AdminUserDTO user, string userRestorer)
        {
            var adminUser = await _proctoringContext.ProctoringAdminUsers
                            .Where(u => u.Login == user.Login)
                            .Where(IsDeleted)
                            .FirstOrDefaultAsync();
            if (adminUser is null)
                return null;

            adminUser.Password = user.Password;
            adminUser.Salt = user.Salt;
            adminUser.UserType = (int)user.UserType;
            adminUser.IsActive = user.IsActive;
            adminUser.PasswordExpirationDate = user.PasswordExpirationDate;

            adminUser.UserDelete = null;
            adminUser.DateDelete = null;
            adminUser.UserUpdate = userRestorer;
            adminUser.DateUpdate = DateTime.UtcNow;

            await _proctoringContext.SaveChangesAsync();

            return adminUser.ToDto();
        }

        public async Task<int> DeleteUserAsync(int userId, string userDeletor)
        {
            var adminUser = await _proctoringContext.ProctoringAdminUsers
                            .Where(user => user.Id == userId)
                            .Where(IsntDeleted)
                            .FirstOrDefaultAsync();

            if (adminUser is null)
                throw new ArgumentException($"Admin user with id {userId} not found.");

            adminUser.UserDelete = userDeletor;
            adminUser.DateDelete = DateTime.UtcNow;

            await _proctoringContext.SaveChangesAsync();

            return userId;
        }

        public async Task<AdminUserDTO> GetUserAsync(string login, bool deleted = false)
        {
            var adminUser = await _proctoringContext.ProctoringAdminUsers.AsNoTracking()
                            .Where(user => user.Login == login)
                            .Where(user => user.IsActive)
                            .Where(deleted ? IsDeleted : IsntDeleted)
                            .FirstOrDefaultAsync();

            return adminUser.ToDto();
        }

        public async Task<AdminUserDTO> GetUserByIdAsync(int userId)
        {
            var adminUser = await _proctoringContext.ProctoringAdminUsers.AsNoTracking()
                            .Where(user => user.Id == userId)
                            .Where(user => user.IsActive)
                            .Where(IsntDeleted)
                            .FirstOrDefaultAsync();

            if (adminUser is null)
                throw new ArgumentException($"no admin with id {adminUser.Id}");
            else
                return adminUser.ToDto();
        }

        public async Task<int> GetCountAdminUsersFilteredAsync(string searchedString = null)
        {
            var adminUsersQuery = GetAdminUsersFilteredQuery(searchedString);
            return await adminUsersQuery.CountAsync();
        }

        public async Task<IEnumerable<AdminUserDTO>> GetAdminUsersFilteredPaginatedAsync(int PageSize, int PageIndex,
            string searchedString = null, string orderByPropertyName = "Id", SortDirection orderByDirection = SortDirection.OrderByDescending)
        {
            orderByPropertyName ??= "Id";
            // Check that the provided property name to use to order results belongs to ProctoringAdminUser entity (using reflexion)
            if (typeof(ProctoringAdminUser).GetProperty(orderByPropertyName) is null)
            {
                // Try to uppercase first letter to handle json formated properties
                orderByPropertyName = orderByPropertyName.Substring(0, 1).ToUpper() + orderByPropertyName.Substring(1);
                if (typeof(ProctoringAdminUser).GetProperty(orderByPropertyName) is null)
                    throw new ArgumentException($"Provided property with name: '{orderByPropertyName}' on which sorting should be performed doesn't belong to {nameof(ProctoringAdminUser)} entity nor table");
            }

            // Check page size and page index
            if (PageIndex <= 0) PageIndex = 1;
            if (PageSize <= 0) PageSize = 10000;

            var adminUsersQuery = GetAdminUsersFilteredQuery(searchedString);
            var adminUsersPage = (await adminUsersQuery
                           .OrderByDynamic<ProctoringAdminUser>(orderByPropertyName, orderByDirection)
                           .ThenByDescending(au => au.Id)
                           .Skip((PageIndex - 1) * PageSize)
                           .Take(PageSize)
                           .ToListAsync())
                           .Select(au => au.ToDto());

            return adminUsersPage;
        }

        /// <summary>
        /// GetAdminUsersFilteredQuery
        /// </summary>
        /// <param name="searchedString">Search string to filter records only containing it inside fields.</param>
        /// <returns>Prepared query with specified conditions, which can be used further to perform required selections.</returns>
        private IQueryable<ProctoringAdminUser> GetAdminUsersFilteredQuery(string searchedString = null)
        {
            Expression<Func<ProctoringAdminUser, bool>> SearchForString = (adminUser) => string.IsNullOrWhiteSpace(searchedString) ? true :
                             (adminUser.Login.Contains(searchedString));

            var adminUsersQuery = _proctoringContext.ProctoringAdminUsers
                           .AsNoTracking()
                           .Where(IsntDeleted)
                           .Where(SearchForString);

            return adminUsersQuery;
        }

        private readonly Expression<Func<ProctoringAdminUser, bool>> IsntDeleted = (user) => (user.DateDelete == null || user.DateDelete > DateTime.Now);
        private readonly Expression<Func<ProctoringAdminUser, bool>> IsDeleted = (user) => (user.DateDelete != null && DateTime.Now > user.DateDelete);
    }
}
