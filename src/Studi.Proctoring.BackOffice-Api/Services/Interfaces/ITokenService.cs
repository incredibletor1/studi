using System;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Services
{
    public interface ITokenService
    {
        int GetUserIdFromToken(string token);
        string GetIssuerFromToken(string token);
        string CreateToken(int userId, string urlSchool, TimeSpan lifetimeSpan);
    }
}
