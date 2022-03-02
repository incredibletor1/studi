using Studi.Api.Proctoring.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Studi.Api.Proctoring.Repositories
{
    public interface IUserExamSessionRepository
    {
        Task<UserExamSessionDto> GetUserExamSessionByIdAsync(int id);
        Task<UserExamSessionDto> GetUserExamSessionAsync(int lmsUserId, string evaluationCode, string promotionCode, string filiereCode);
        Task<UserExamSessionDto>  CreateUserExamSessionAsync(UserExamSessionDto userExamSession);
        Task UpdateUserExamSessionAsync(UserExamSessionDto userExamToUpdate);
    }
}
