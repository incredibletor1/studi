using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Repositories.Interfaces
{
    public class PasswordHashPair
    {
        public string HashB64 { set; get; }
        public string SaltB64 { set; get; }
    }

    public interface IPasswordService
    {
        PasswordHashPair GenerateHash(string password);
        string CalculateHash(string password, string salt);
        bool TestPasswordForRequirements(string password);
    }
}
