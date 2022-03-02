using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Studi.Proctoring.BackOffice_Api.Helpers;
using Studi.Proctoring.BackOffice_Api.Models.DTO;
using Studi.Proctoring.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Studi.Proctoring.BackOffice_Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IProctoringContext _proctoringContext;

        public UserRepository(IProctoringContext proctoringContext, IMapper mapper)
        {
            this._proctoringContext = proctoringContext;
            ConversionExtension.InitMapper(mapper);
        }

        /// <summary>
        /// Get a user by its id
        /// </summary>
        /// <param name="id">the user id</param>
        /// <returns>returns the corresponding user, throw exception if not found</returns>
        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _proctoringContext.Users.AsNoTracking()
                .Where(user => user.Id == id)
                .Where(IsntDeleted)
                .FirstOrDefaultAsync();

            return user.ToDto();
        }

        // Helper methods:
        private readonly Expression<Func<User, bool>> IsntDeleted = (user) => (user.DateDelete == null || user.DateDelete > DateTime.Now);
    }
}
