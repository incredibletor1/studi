using Studi.Api.Proctoring.Models.DTO;
using System.Threading.Tasks;

namespace Studi.Api.Proctoring.Services
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> GetUserByLmsUserIdAsync(int lmsUserId);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<UserDto> CreateUserWithInfosFromLmsAsync(string evaluationCode, string authToken, ILmsApiClient lmsApiClient);
    }
}