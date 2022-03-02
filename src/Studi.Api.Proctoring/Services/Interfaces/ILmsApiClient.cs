using Studi.Api.Proctoring.Models.DTO;

namespace Studi.Api.Proctoring.Services
{
    public interface ILmsApiClient
    {
        UserDto GetUserInfos(string evaluationCode);
        ExamSessionDto GetExamInfos(int ressourceVersionId, string promotionCode, string filiereCode);
        UserExamSessionDto GetUserExamInfos(string evaluationCode, string promotionCode, int userId, int examSessionId);
        string LmsApiBaseUrl { get; set; }
        string AuthToken { get; set; }
    }
}
