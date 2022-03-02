using Microsoft.IdentityModel.Tokens;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using System;
using System.Text;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Studi.Proctoring.BackOffice_Api.Models;

namespace Studi.Proctoring.BackOffice_Api.Services
{
    public class AdminTokenService : IAdminTokenService
    {
        private static IOptions<AdminJwtInfo> _config;

        public AdminTokenService(IOptions<AdminJwtInfo> config)
        {
            _config = config;
        }

        public string CreateToken(AdminUserDTO user)
        {
            var now = DateTime.UtcNow;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Login),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
            };

            var jwt = new JwtSecurityToken(
                    issuer: _config.Value.issuer,
                    notBefore: now,
                    claims: claims,
                    expires: now.Add(TimeSpan.FromMinutes(_config.Value.lifetime)),
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(System.Convert.FromBase64String(_config.Value.keyString)),
                        SecurityAlgorithms.HmacSha256));

            string token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return System.Convert.ToBase64String(Encoding.ASCII.GetBytes(token));
        }

        public string GetUserLogin(ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Email);
        }
    }
}
