using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Studi.Api.Proctoring.Helpers;
using Studi.Api.Proctoring.Models.DTO;
using Studi.Proctoring.Database.Context;

namespace Studi.Api.Proctoring.Repositories
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
        /// Create user by user model
        /// </summary>
        /// <param name="user">the user model</param>
        public async Task<UserDto> CreateUserAsync(UserDto user)
        {
            var userEntity = user.ToEntity();
            userEntity.DateCreate = DateTime.Now;
            userEntity.UserCreate = GlobalConst.ProctoringApiUserName;
            //
            await _proctoringContext.Users.AddAsync(userEntity);
            await _proctoringContext.SaveChangesAsync();
            return userEntity.ToDto();
        }

        

        /// <summary>
        /// Get a user by its email
        /// </summary>
        /// <param name="email">the user email</param>
        /// <returns>returns the corresponding user, throw exception if not found</returns>
        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _proctoringContext.Users.AsNoTracking()
                .Where(user => user.Email == email)
                .Where(IsntDeleted)
                .FirstOrDefaultAsync();
            return user.ToDto();
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

        /// <summary>
        /// Get a user by its id in LMS
        /// </summary>
        /// <param name="lmsUserId"></param>
        /// <returns></returns>
        public async Task<UserDto> GetUserByLmsUserIdAsync(int lmsUserId)
        {
            var user = await _proctoringContext.Users.AsNoTracking()
                .Where(user => user.LmsUserId == lmsUserId)
                .Where(IsntDeleted)
                .FirstOrDefaultAsync();
            return user.ToDto();
        }

        public IEnumerable<UserDto> GetUsersFiltered(Expression<Func<User, bool>> filterPredicate)
        {
            var users = _proctoringContext.Users.AsNoTracking()
                .Where(IsntDeleted)
                .Where(filterPredicate)
                .ToList();
            return users.Select(user => user.ToDto());
        }

        // Helper methods:
        private readonly Expression<Func<User, bool>> IsntDeleted = (user) => (user.DateDelete == null || user.DateDelete > DateTime.Now);
    }
}
