using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Studi.Proctoring.BackOffice_Api.Models;
using Studi.Proctoring.BackOffice_Api.Repositories.Interfaces;

namespace Studi.Proctoring.BackOffice_Api.Repositories
{
    public class PasswordService : IPasswordService
    {
        private static IOptions<PasswordHashConfig> _config;

        public PasswordService(IOptions<PasswordHashConfig> config)
        {
            _config = config;
        }

        public PasswordHashPair GenerateHash(string password)
        {
            var genKeyObj = new Rfc2898DeriveBytes(password, _config.Value.SaltSize, _config.Value.Iterations);
            string passKey = Convert.ToBase64String(genKeyObj.GetBytes(_config.Value.HashSize));
            string salt = Convert.ToBase64String(genKeyObj.Salt);
            return new PasswordHashPair
            {
                HashB64 = passKey,
                SaltB64 = salt
            };
        }

        public string CalculateHash(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var genKeyObj = new Rfc2898DeriveBytes(password, saltBytes, _config.Value.Iterations);
            return Convert.ToBase64String(genKeyObj.GetBytes(_config.Value.HashSize));
        }
        public bool TestPasswordForRequirements(string password)
        {
            string regex = "^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@$!%*#?&]).{8,30}$";
            var match = Regex.Match(password, regex);
            return match.Success;
        }
    }
}
