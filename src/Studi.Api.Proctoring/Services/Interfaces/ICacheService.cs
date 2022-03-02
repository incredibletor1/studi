using System;
using System.Threading.Tasks;

namespace Studi.Api.Proctoring.Services
{
    public interface ICacheService
    {
        Task<int> GetUserIdByLmsUserIdAsync(int lmsUserId);
        void RemoveFromCacheUserIdByLmsUserId(int lmsUserId);
        Task<int> GetUserExamSessionIdIfOngoingAsync(int lmsUserId, string evaluationCode, string promotionCode, string filiereCode);
        void RemoveFromCacheUserExamSessionId(int lmsUserId, string evaluationCode, string promotionCode, string filiereCode);
        Task<int> GetImageMaxOrderByUserExamId(int userExamSessionId);
        Task UpdateCacheImageMaxOrderByUserExamId(int userExamSessionId, int nextOrder);
        void RemoveFromCacheImageMaxOrderByUserExamId(int userExamSessionId);
        Task<DateTime> GetLastImageTimestampByUserExamId(int userExamSessionId);
        Task UpdateCacheLastImageTimestampByUserExamId(int userExamSessionId, DateTime imageTimestamp);
    }
}