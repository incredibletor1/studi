using Studi.Proctoring.BackOffice_Api.Models;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Repositories
{
    public interface IAdminUserRepository
    {
        public Task<AdminUserDTO> CreateUserAsync(AdminUserDTO user, string userCreator);
        public Task<AdminUserDTO> RestoreUserAsync(AdminUserDTO user, string userRestorer);
        public Task<int> DeleteUserAsync(int userId, string userDeletor);

        public Task<AdminUserDTO> GetUserAsync(string login, bool deleted = false);
        public Task<AdminUserDTO> GetUserByIdAsync(int userId);
        public Task<int> GetCountAdminUsersFilteredAsync(string searchedString = null);
        public Task<IEnumerable<AdminUserDTO>> GetAdminUsersFilteredPaginatedAsync(int PageSize, int PageIndex,
            string searchedString = null, string orderByPropertyName = "Id", SortDirection orderByDirection = SortDirection.OrderByDescending);
    }
}
