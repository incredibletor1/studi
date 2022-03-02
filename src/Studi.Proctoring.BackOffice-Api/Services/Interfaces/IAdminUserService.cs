using Studi.Proctoring.BackOffice_Api.Models;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Studi.Proctoring.BackOffice_Api.Models.DTO.AdminUserDTO;

namespace Studi.Proctoring.BackOffice_Api.Services
{
    public interface IAdminUserService
    {
        public Task<AdminUserDTO> LoginUserAsync(string login, string password);

        public Task<AdminUserDTO> GetUserAsync(string login);
        public Task<AdminUserDTO> GetUserByIdAsync(int userId);
        public Task<AdminUsersPageDTO> GetAdminUsersFilteredPaginatedAsync(int PageSize, int PageIndex, string searchedString = null, string orderByPropertyName = "Id", SortDirection orderByDirection = SortDirection.OrderByDescending);
        public Task<AdminUserDTO> CreateUserAsync(string login, string password, string userCreator);
        public Task<int> DeleteUserAsync(int userId, string userDeletor);
        public Task<List<int>> DeleteUsersAsync(List<int> userIds, string userDeletor);
    }
}
