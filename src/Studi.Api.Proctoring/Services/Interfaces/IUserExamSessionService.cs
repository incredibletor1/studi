using Studi.Api.Proctoring.Models.DTO;
using Studi.Proctoring.Database.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Studi.Api.Proctoring.Services
{
    public interface IUserExamSessionService
    {
        Task<UserExamSessionDto> GetUserExamSessionByIdAsync(int id);
        Task<UserExamSessionDto> GetUserExamSessionAsync(int lmsUserId, string evaluationCode, string promotionCode, string filiereCode);
        Task<UserExamSessionDto> CreateUserExamSessionWithInfosFromLmsAsync(int lmsUserId, ExamSessionDto examSession, IDTypeEnum? IDimageType, bool hasUserPictureBeenProvided, bool hasUserIdentityDocBeenProvided, string authToken, ILmsApiClient lmsApiClient);
        Task EndUserExamSessionAsync(int userExamSessionId);
        Task<UserExamSessionDto> UpdateMaterialCheckAndStartUserExamAsync(UserExamMaterialCheckDto userExamMaterialCheckDto);
    }
}
