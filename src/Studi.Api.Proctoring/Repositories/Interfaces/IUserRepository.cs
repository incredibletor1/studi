using Studi.Api.Proctoring.Models.DTO;
using System.Threading.Tasks;

namespace Studi.Api.Proctoring.Repositories
{
    public interface IUserRepository
    {
        Task<UserDto> CreateUserAsync(UserDto user);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> GetUserByLmsUserIdAsync(int lmsUserId);
    }
}