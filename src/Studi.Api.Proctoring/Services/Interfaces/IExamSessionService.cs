using Studi.Api.Proctoring.Middlewares.Exceptions;
using Studi.Api.Proctoring.Models;
using Studi.Api.Proctoring.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Studi.Api.Proctoring.Services
{
    public interface IExamSessionService
    {
        Task<ExamSessionDto> GetExamSessionByIdAsync(int id);
        Task<ExamSessionDto> GetExamSessionAsync(string evaluationCode, string promotionCode, string filiereCode);
        Task<ExamSessionDto> CreateExamSessionWithInfosFromLmsAsync(int ressourceVersionId, string promotionCode, string filiereCode, string authToken, ILmsApiClient lmsApiClient);
    }
}
