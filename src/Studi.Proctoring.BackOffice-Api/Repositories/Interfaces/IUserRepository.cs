using Studi.Proctoring.BackOffice_Api.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Repositories
{
    public interface IUserRepository
    {
        Task<UserDto> GetUserByIdAsync(int id);
    }
}
