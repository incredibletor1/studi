using Studi.Proctoring.BackOffice_Api.Models.DTO;
using System.Security.Claims;

namespace Studi.Proctoring.BackOffice_Api.Services
{
    public interface IAdminTokenService
    {
        public string CreateToken(AdminUserDTO user);
        public string GetUserLogin(ClaimsPrincipal user);
    }
}
